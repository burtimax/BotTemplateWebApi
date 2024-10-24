using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Options;
using BotFramework.Repository;
using BotFramework.Utils.ReportGenerator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using File = System.IO.File;

namespace BotFramework.Utils.ExceptionHandler;

/// <summary>
/// Обработчик ошибок бота.
/// </summary>
/// <remarks>
/// Уведомляет об ошибках модераторов.
/// Сохраняет ошибки в БД и в директории.
/// </remarks>
public class BotExceptionHandler
{
    /// <summary>
    /// 1) Отправляет отчет об ошибке тех поддержке в бота.
    /// 2) Сохраняет ошибку в БД.
    /// 3) Сохраняет файл ошибки в файловое хранилище.
    /// </summary>
    /// <param name="e">Ошибка программы.</param>
    /// <param name="args">Параметры метода.</param>
    public async Task Handle(BotExceptionHandlerArgs args, CancellationToken? cancellationToken = null)

    {
        Exception e = args.Exception;
        Update? update = args.TelegramUpdate;
        BotUpdate? botUpdate = args.BotUpdate;
        BotUser? user = args.BotUser;
        BotChat? chat = args.BotChat;
        IServiceProvider serviceProvider = args.ServiceProvider;

        // Получим сервисы из DI
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        IBotUpdateRepository botUpdateRepository = serviceProvider.GetRequiredService<IBotUpdateRepository>();
        BotDbContext db = serviceProvider.GetRequiredService<BotDbContext>();
        BotConfiguration botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        BotOptions botOptions = serviceProvider.GetRequiredService<IOptions<BotOptions>>()?.Value ?? new();

        ExceptionMessageReportGenerator reportGenerator = new();
        string messageReport =
            await reportGenerator.GenerateExceptionMessage(e, update, user, chat, botUpdateRepository);

        using (Stream stream = StreamHelper.GenerateStreamFromString(messageReport))
        {
            string errorFileName = $"{DateTime.Now.Ticks.ToString()}.txt";
            InputFileStream fileException = InputFile.FromStream(stream, errorFileName);
            StringBuilder captionBuilder = new();
            captionBuilder.AppendLine($"<b>Ошибка программы</b>")
                .AppendLine();

            if (user is not null)
            {
                captionBuilder.AppendLine($"<b>User.Id</b>: {user.Id}")
                    .AppendLine($"<b>User.TelegramId</b>: <code>{user.TelegramId}</code>")
                    .AppendLine($"<b>User.Username</b>: {user.GetUsernameLink() ?? "NULL"}")
                    .AppendLine($"<b>User.Firstname</b>: {user.TelegramFirstname ?? "NULL"}")
                    .AppendLine($"<b>User.Lastname</b>: {user.TelegramLastname ?? "NULL"}")
                    .AppendLine();
            }

            if (chat is not null)
            {
                captionBuilder.AppendLine($"<b>Chat.Id</b>: {chat.Id}")
                    .AppendLine($"<b>Chat.TelegramChatId</b>: <code>{chat.ChatId}</code>")
                    .AppendLine($"<b>Chat.CurrentState</b>: {chat.States.ToString()}")
                    .AppendLine();
            }

            captionBuilder.AppendLine($"<b>DateTime</b>: {DateTime.Now.ToString()}");

            string caption = captionBuilder.ToString();

            if (caption.Length > BotConstants.Constraints.MaxDocumentCaption)
            {
                caption = caption.Substring(0, BotConstants.Constraints.MaxDocumentCaption);
            }

            // Отправляем модераторам и админу.
            IEnumerable<long> moderatorUserIds = await db.UserClaims.Include(uc => uc.Claim)
                .Where(uc =>
                    uc.Claim.Name == BotConstants.BaseBotClaims.BotExceptionsGet ||
                    uc.Claim.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty)
                .Select(uc => uc.UserId)
                .ToListAsync();
            IEnumerable<BotChat> moderatorChats =
                await db.Chats.Where(c => moderatorUserIds.Contains(c.BotUserId)).ToListAsync();

            InputFile fileFromTelegram = null;
            
            foreach (BotChat ch in moderatorChats)
            {
                try
                {
                    if (cancellationToken?.IsCancellationRequested == true) return;
                    
                    var message = await botClient.SendDocumentAsync(ch.ChatId, fileFromTelegram ?? fileException, caption: caption,
                        parseMode: ParseMode.Html);
                    
                    // Другим отрпавляем тот же документ, только по ИД.
                    fileFromTelegram = InputFile.FromString(message.Document!.FileId);
                }
                catch (Exception exception)
                {
                    // Не смогли отправить сообщение в Telegram.
                }
            }

            // Добавляем в БД.
            if (cancellationToken?.IsCancellationRequested == true) return;
            
            DateTimeOffset now = DateTime.Now;
            string exceptionFileName = $"{botConfig.Name} {now.ToString("yyyy.MM.dd hh.mm.ss")}.txt";

            if (botOptions.SaveExceptionsInDatabase)
            {
                BotException botException = new BotException()
                {
                    ChatId = chat?.Id,
                    UserId = user?.Id,
                    UpdateId = botUpdate?.Id,
                    CreatedAt = now,
                    ExceptionMessage = e.Message,
                    StackTrace = e.StackTrace,
                    ReportDescription = messageReport,
                    ReportFileName = exceptionFileName,
                };
                db.Exceptions.Add(botException);
                await db.SaveChangesAsync();
            }

            // Сохраняем в файл если указанная директория существует.
            if (cancellationToken?.IsCancellationRequested == true) return;
            
            if (botOptions.SaveExceptionsInDirectory && Directory.Exists(botConfig.ExceptionDirectory))
            {
                await File.WriteAllTextAsync(Path.Combine(botConfig.ExceptionDirectory, exceptionFileName),
                    messageReport);
            }
        }
    }
}