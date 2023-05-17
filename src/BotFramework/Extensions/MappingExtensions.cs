using BotFramework.Db.Entity;
using Telegram.Bot.Types;

namespace BotFramework.Extensions
{
    /// <summary>
    /// Методы для моделей TelegramClient библиотеки
    /// </summary>
    public static class MappingExtensions
    {
        public static BotUser ToBotUserEntity(this User user, string role = null, string status = null)
        {
            if (string.IsNullOrEmpty(role))
            {
                role = BotConstants.UserRoles.User;
            }
            
            return new BotUser()
            {
                TelegramFirstname = user.FirstName,
                TelegramLastname = user.LastName,
                TelegramId = user.Id,
                TelegramUsername = user.Username,
                Role = role,
                Status = status
            };
        }
        
        public static BotChat ToBotChatEntity(this Chat chat, long botUserId)
        {
            return new BotChat()
            {
                TelegramId = chat.Id,
                TelegramUsername = chat.Username,
                BotUserId = botUserId,
            };
        }
    }
}