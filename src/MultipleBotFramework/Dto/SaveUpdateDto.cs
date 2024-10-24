namespace MultipleBotFramework.Dto
{
    /// <summary>
    /// Dto для сохранения сообщения.
    /// </summary>
    public class SaveUpdateDto
    {
        /// <summary>
        /// Идентификатор сущности чата, которому принадлежит сообщение.
        /// Внешний ключ для сообщения. 
        /// </summary>
        public long? BotChatId { get; set; }
        
        /// <summary>
        /// Идентификатор сущности чата, которому принадлежит обновление.
        /// </summary>
        public long? BotUserId { get; set; }

        /// <summary>
        /// Telegram идентификатор сообщения.
        /// </summary>
        public long TelegramId { get; set; }

        /// <summary>
        /// Тип сообщения.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Содержимое сообщения.
        /// </summary>
        /// <remarks>Текст, ссылка на файл и т.д.</remarks>
        public string Content { get; set; }
    }
}