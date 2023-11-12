using System;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Сообщение пользователя
    /// </summary>
    [Comment("Таблица сообщений (запросов) бота.")]
    public class BotUpdate : BaseBotEntity<Guid>
    {
        /// <summary>
        /// Внешний ключ на сущность BotChat
        /// </summary>
        [Comment("Внешний ключ на таблицу чатов.")]
        public long BotChatId { get; set; }
        public BotChat Chat { get; set; }
        
        /// <summary>
        /// Telegram Message Id
        /// </summary>
        [Comment("Идентификатор сообщения в Telegram чате.")]
        public long TelegramMessageId { get; set; }
        
        /// <summary>
        /// Тип сообщения (текст, картинка, аудио, документ и т.д.)
        /// </summary>
        [Comment("Тип сообщения (текст, картинка, аудио, документ и т.д.).")]
        public string Type { get; set; }

        /// <summary>
        /// Содержание сообщения (текст или имя файла)
        /// </summary>
        [Comment("Содержимое сообщения.")]
        public string Content { get; set; }
    }
}