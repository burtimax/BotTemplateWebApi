using System;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Enums;

namespace MultipleBotFramework.Db.Entity
{
    /// <summary>
    /// Сообщение пользователя
    /// </summary>
    [Comment("История чата.")]
    public class BotChatHistoryEntity : BaseEntity<long>
    {
        /// <summary>
        /// Внешний ключ на сущность BotChat
        /// </summary>
        [Comment("ИД чата в телеграме.")]
        public long TelegramChatId { get; set; }

        [Comment("Принадлежность сообщения определенной медиагруппе.")]
        public string? MediaGroupId { get; set; }
        
        /// <summary>
        /// Telegram Message Id
        /// </summary>
        [Comment("Идентификатор сообщения в Telegram чате.")]
        public long? MessageId { get; set; }
        
        /// <summary>
        /// Тип сообщения.
        /// </summary>
        [Comment("Идентификатор сообщения в Telegram чате.")]
        public string? MessageType { get; set; }
        
        /// <summary>
        /// Тип сообщения (текст, картинка, аудио, документ и т.д.)
        /// </summary>
        [Comment("Тип элемента истории: message, callback, membertype and ect...")]
        public ChatHistoryType Type { get; set; }

        /// <summary>
        /// Строковое представление содержимого сообщения.
        /// </summary>
        [Comment("Строковое представление содержимого сообщения.")]
        public string? Content { get; set; }
        
        /// <summary>
        /// JSON представление содержимого сообщения.
        /// </summary>
        [Comment("JSON представление содержимого сообщения.")]
        public string? JsonObject { get; set; }
        
        /// <summary>
        /// Заполняется при наличии файла в сообщении.
        /// </summary>
        /// <returns></returns>
        [Comment("Заполняется при наличии файла в сообщении.")]
        public string? FileId { get; set; }

        /// <summary>
        /// Кто отправил сообщение: бот или пользователь.
        /// </summary>
        [Comment("Кто отправил сообщение: бот или пользователь.")]
        public bool IsBot { get; set; }

        /// <summary>
        /// Просмотрено модератором.
        /// </summary>
        [Comment("Просмотрено модератором.")]
        public bool IsViewed { get; set; } = false;
        
        /// <summary>
        /// Внешний ключ на бота.
        /// </summary>
        [Comment("Внешний ключ на бота.")]
        public long BotId { get; set; }
        public BotEntity? Bot { get; set; }
    }
}