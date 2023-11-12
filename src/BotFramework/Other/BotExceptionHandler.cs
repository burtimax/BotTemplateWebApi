using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Models;
using BotFramework.Options;
using BotFramework.Other.ReportGenerator;
using BotFramework.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace BotFramework.Other;

/// <summary>
/// Обработчик ошибок.
/// </summary>
public class BotExceptionHandler
{
    /// <summary>
    /// Отправляет отчет об ошибке тех поддержке.
    /// </summary>
    /// <param name="e"></param>
    public async Task Handle(Exception e, Update update, BotUpdate botUpdate, BotUser user, BotChat chat, IServiceProvider serviceProvider)
    {
        // Получим сервисы из DI
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        IBotUpdateRepository botUpdateRepository = serviceProvider.GetRequiredService<IBotUpdateRepository>();
        BotDbContext db = serviceProvider.GetRequiredService<BotDbContext>();
        BotConfiguration botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        BotOptions botOptions = serviceProvider.GetRequiredService<IOptions<BotOptions>>()?.Value ?? new();
            
        ExceptionMessageReportGenerator reportGenerator = new();
        string messageReport = await reportGenerator.GenerateExceptionMessage(e, update, user, chat, botUpdateRepository);

        using (Stream stream = StreamHelper.GenerateStreamFromString(messageReport))
        {
            string errorFileName = $"{DateTime.Now.Ticks.ToString()}.txt";
            InputOnlineFile fileException = new InputOnlineFile(stream, errorFileName);
            StringBuilder captionBuilder = new();
            captionBuilder.AppendLine($"<b>Ошибка программы</b>")
                .AppendLine()
                .AppendLine($"<b>User.Id</b>: {user.Id}")
                .AppendLine($"<b>User.TelegramId</b>: <code>{user.TelegramId}</code>")
                .AppendLine($"<b>User.Username</b>: {user.GetUsernameLink() ?? "NULL"}")
                .AppendLine($"<b>User.Firstname</b>: {user.TelegramFirstname ?? "NULL"}")
                .AppendLine($"<b>User.Lastname</b>: {user.TelegramLastname ?? "NULL"}")
                .AppendLine()
                .AppendLine($"<b>Chat.Id</b>: {chat.Id}")
                .AppendLine($"<b>Chat.TelegramChatId</b>: <code>{chat.ChatId}</code>")
                .AppendLine($"<b>Chat.CurrentState</b>: {chat.States.ToString()}")
                .AppendLine()
                .AppendLine($"<b>DateTime</b>: {DateTime.Now.ToString()}");

            string caption = captionBuilder.ToString();

            if (caption.Length > BotConstants.Constraints.MaxDocumentCaption)
            {
                caption = caption.Substring(0, BotConstants.Constraints.MaxDocumentCaption);
            }
            
            // Отправляем модераторам и админу.
            IEnumerable<long> moderatorUserIds = await db.UserClaims.Include(uc => uc.Claim)
                .Where(uc => uc.Claim.Name == BotConstants.BaseBotClaims.BotExceptionsGet || uc.Claim.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty)
                .Select(uc => uc.UserId)
                .ToListAsync();
            IEnumerable<BotChat> moderatorChats =
                await db.Chats.Where(c => moderatorUserIds.Contains(c.BotUserId)).ToListAsync();

            foreach (BotChat ch in moderatorChats)
            {
                await botClient.SendDocumentAsync(ch.ChatId, fileException, caption: caption, parseMode: ParseMode.Html);
            }

            // Добавляем в БД.
            DateTimeOffset now = DateTime.Now;
            string exceptionFileName = $"{botConfig.Name} {now.ToString("yyyy.MM.dd hh.mm.ss")}.txt";
            
            if (botOptions.SaveExceptionsInDatabase)
            {
                BotException botException = new BotException()
                {
                    ChatId = chat.Id,
                    UserId = user.Id,
                    UpdateId = botUpdate.Id,
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
            if (botOptions.SaveExceptionsInDirectory && Directory.Exists(botConfig.ExceptionDirectory))
            {
                await File.WriteAllTextAsync(Path.Combine(botConfig.ExceptionDirectory, exceptionFileName), messageReport);
            }
        }
    }
}