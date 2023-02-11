using System;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Сообщение пользователя
    /// </summary>
    public class BotMessage : BaseBotEntity<Guid>
    {
        /// <summary>
        /// Внешний ключ
        /// </summary>
        public long BotChatId { get; set; }
        public BotChat Chat { get; set; }
        
        /// <summary>
        /// Telegram Message Id
        /// </summary>
        public long TelegramMessageId { get; set; }
        
        /// <summary>
        /// Тип сообщения (текст, картинка, аудио, документ и т.д.)
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Содержание сообщения (текст или имя файла)
        /// </summary>
        public string Content { get; set; }
    }
}