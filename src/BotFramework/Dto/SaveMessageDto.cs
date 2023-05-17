using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BotFramework.Dto
{
    /// <summary>
    /// Dto для сохранения сообщения.
    /// </summary>
    public class SaveMessageDto
    {
        /// <summary>
        /// Идентификатор сущности чата, которому принадлежит сообщение.
        /// Внешний ключ для сообщения. 
        /// </summary>
        public long BotChatId { get; set; }

        /// <summary>
        /// Telegram идентификатор сообщения.
        /// </summary>
        public long TelegramId { get; set; }

        /// <summary>
        /// Тип сообщения.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Содержимое сообщения.
        /// </summary>
        /// <remarks>Текст, ссылка на файл и т.д.</remarks>
        public string MessageContent { get; set; }
    }
}