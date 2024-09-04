using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Constants;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Repository;
using MultipleBotFrameworkUpgrade.Services;
using MultipleBotFrameworkUpgrade.Utils.ReportGenerator;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using File = System.IO.File;

namespace MultipleBotFrameworkUpgrade.Utils.ExceptionHandler;

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
        BotUpdateEntity? botUpdate = args.BotUpdate;
        BotUserEntity? user = args.BotUser;
        BotChatEntity? chat = args.BotChat;
        IServiceProvider serviceProvider = args.ServiceProvider;
        long botId = args.BotId;

        // Получим сервисы из DI
        IBotFactory botFactory = serviceProvider.GetRequiredService<IBotFactory>();
        ITelegramBotClient? botClient = (botFactory.GetInstance(botId));
        if(botClient is null) return;
        
        IBotUpdateRepository botUpdateRepository = serviceProvider.GetRequiredService<IBotUpdateRepository>();
        BotDbContext db = serviceProvider.GetRequiredService<BotDbContext>();
        BotConfiguration botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        BotOptions botOptions = serviceProvider.GetRequiredService<IOptions<BotOptions>>()?.Value ?? new();

        ExceptionMessageReportGenerator reportGenerator = new();
        string messageReport =
            await reportGenerator.GenerateExceptionMessage(e, botId, update, user, chat, botUpdateRepository);

        using (Stream stream = StreamHelper.GenerateStreamFromString(messageReport))
        {
            string errorFileName = $"{DateTime.Now.Ticks.ToString()}.txt";
            InputFile fileException = new InputFile(stream, errorFileName);
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
            IEnumerable<BotChatEntity> moderatorChats =
                await db.Chats.Where(c => c.BotUserId != null && moderatorUserIds.Contains(c.BotUserId.Value)).ToListAsync();

            string fileFromTelegram = null;
            
            foreach (BotChatEntity ch in moderatorChats)
            {
                try
                {
                    if (cancellationToken?.IsCancellationRequested == true) return;

                    Message message = null;
                    if (string.IsNullOrEmpty(fileFromTelegram) == false)
                    {
                        message = await botClient.SendDocumentAsync(ch.ChatId, fileFromTelegram, caption: caption,
                            parseMode: ParseMode.Html);
                    }
                    else
                    {
                        message = await botClient.SendDocumentAsync(ch.ChatId, fileException, caption: caption,
                            parseMode: ParseMode.Html);
                    }
                    
                    // Другим отрпавляем тот же документ, только по ИД.
                    fileFromTelegram = message.Document!.FileId;
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
                BotExceptionEntity botExceptionEntity = new BotExceptionEntity()
                {
                    ChatEntityId = chat?.Id,
                    UserEntityId = user?.Id,
                    UpdateEntityId = botUpdate?.Id,
                    CreatedAt = now,
                    ExceptionMessage = e.Message,
                    StackTrace = e.StackTrace,
                    ReportDescription = messageReport,
                    ReportFileName = exceptionFileName,
                };
                db.Exceptions.Add(botExceptionEntity);
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