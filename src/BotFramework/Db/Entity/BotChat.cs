using System.ComponentModel.DataAnnotations.Schema;
using Telegram.Bot.Types;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Сущность чата
    /// </summary>
    public class BotChat : BaseBotEntity<long>
    {
        /// <summary>
        /// Идентификатор чата в телеграм
        /// </summary>
        public long? TelegramId { get; set; }

        /// <summary>
        /// Идентификатор чата в телеграм
        /// </summary>
        /// <remarks>Некоторые чаты вместо long идентификатора имеют username идентификатор</remarks>
        public string? TelegramUsername { get; set; }

        /// <summary>
        /// Получить Telegram идентификатор чата (Id или Username)
        /// </summary>
        /// <remarks>NotMapped свойство</remarks>
        [NotMapped]
        public ChatId ChatId {
            get
            {
                return TelegramId != null ? 
                    new ChatId((long)TelegramId) : 
                    new ChatId(TelegramUsername ?? "");
            }
        }

        /// <summary>
        /// Внешний ключ на пользователя
        /// </summary>
        public long BotUserId { get; set; }
        public BotUser BotUser { get; set; }

        /// <summary>
        /// Состояние чата 
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Хранилище данных чата
        /// </summary>
        public string DataStore { get; set; }
    }
}