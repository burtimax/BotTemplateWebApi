using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Repository;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

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
    public async Task Handle(Exception e, Update update, BotUser user, BotChat chat, IServiceProvider serviceProvider)
    {
        // Получим сервисы из DI
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        IBotUpdateRepository botUpdateRepository = serviceProvider.GetRequiredService<IBotUpdateRepository>();
            
        ExceptionMessageGenerator generator = new();
        string messageReport = await generator.GenerateExceptionMessage(e, update, user, chat, botUpdateRepository);

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

            if (caption.Length > BotConstants.Bounds.MaxDocumentCaption)
            {
                caption = caption.Substring(0, BotConstants.Bounds.MaxDocumentCaption);
            }
            
            await botClient.SendDocumentAsync(chat.ChatId, fileException, caption: caption, parseMode: ParseMode.Html);
        }
    }
}