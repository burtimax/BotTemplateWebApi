using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Repository;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace BotFramework.Utils.ReportGenerator;

/// <summary>
/// Генератор сообщений об ошибках.
/// </summary>
public class ExceptionMessageReportGenerator
{
    public async Task<string> GenerateExceptionMessage(Exception e, Update? update, BotUser? user, BotChat? chat, IBotUpdateRepository? updateRepository)
    {
        StringBuilder sb = new();
        sb.AppendLine($"# ERROR")
            .AppendLine()
            .AppendLine($"InputTelegramUpdate:")
            .AppendLine(JsonConvert.SerializeObject(update, Formatting.Indented))
            .AppendLine()
            .AppendLine($"ErrorMessage:")
            .AppendLine(e.Message)
            .AppendLine()
            .AppendLine($"StackTrace:")
            .AppendLine(e.StackTrace)
            .AppendLine()
            .AppendLine($"Source:")
            .AppendLine(e.Source)
            .AppendLine();
        
        if (user is not null)
        {
            sb.AppendLine($"### User")
                .AppendLine($"UserId: {user.Id}")
                .AppendLine($"UserRole: {user.Role ?? "NULL"}")
                .AppendLine($"UserTelegramId: {user.TelegramId}")
                .AppendLine($"Username: {user.TelegramUsername ?? "NULL"}")
                .AppendLine($"TelegramFirstname: {user.TelegramFirstname ?? "NULL"}")
                .AppendLine($"TelegramLastname: {user.TelegramLastname ?? "NULL"}")
                .AppendLine();
        }

        if (chat is not null)
        {
            sb.AppendLine($"### Chat")
                .AppendLine($"ChatId: {chat.Id}")
                .AppendLine($"TelegramChatId: {chat.ChatId}")
                .AppendLine($"CurrentState: {chat.States.ToString()}")
                .AppendLine($"Data: {chat.Data.ToString()}")
                .AppendLine();

            if (updateRepository is not null)
            {
                IEnumerable<BotUpdate> LastMessages = await updateRepository.GetLastCountChatMessages(chat.Id, 5);

                sb.AppendLine($"### Last messages")
                    .AppendLine();

                foreach (var message in LastMessages)
                {
                    sb.AppendLine($"Message: {message.Content ?? "NULL"}")
                        .AppendLine($"MessageType: {message.Type ?? "NULL"}")
                        //.AppendLine($"MessageChatState: {message.}")
                        .AppendLine($"DateTime: {message.CreatedAt}")
                        .AppendLine();
                }
            }
        }

        return sb.ToString();
    }
}