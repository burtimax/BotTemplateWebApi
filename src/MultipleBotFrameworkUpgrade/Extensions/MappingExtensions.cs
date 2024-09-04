using MultipleBotFrameworkUpgrade.Db.Entity;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFrameworkUpgrade.Extensions
{
    /// <summary>
    /// Методы для моделей TelegramClient библиотеки
    /// </summary>
    public static class MappingExtensions
    {
        public static BotUserEntity ToBotUserEntity(this User user, long botId, string role = null, string status = null)
        {
            if (string.IsNullOrEmpty(role))
            {
                role = BotConstants.UserRoles.User;
            }
            
            return new BotUserEntity()
            {
                BotId = botId,
                TelegramFirstname = user.FirstName,
                TelegramLastname = user.LastName,
                TelegramId = user.Id,
                TelegramUsername = user.Username,
                Role = role,
                LanguageCode = user.LanguageCode,
            };
        }
        
        public static BotChatEntity ToBotChatEntity(this Chat chat, long botId, long? botUserId)
        {
            return new BotChatEntity()
            {
                BotId = botId,
                TelegramId = chat.Id,
                TelegramUsername = chat.Username,
                BotUserId = botUserId,
                Type = chat.Type,
            };
        }
    }
}