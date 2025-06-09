namespace MultipleBotFramework
{
    /// <summary>
    /// Класс, содержащий константы и статические значения, используемые в бот-фреймворке
    /// </summary>
    public partial class BotConstants
    {
        /// <summary>
        /// Начальное состояние бота при старте
        /// </summary>
        public const string StartState = "StartState";
      
        /// <summary>
        /// Формат ссылки на Telegram пользователя
        /// Используется для генерации ссылок вида https://t.me/{USERNAME}
        /// </summary>
        public const string TelegramLinkFormat = "https://t.me/{0}";
        
        /// <summary>
        /// Класс, содержащий теги для модерации чатов
        /// </summary>
        public class ModerationChatTags
        {
            /// <summary>
            /// Тег для сообщений об исключениях
            /// </summary>
            public const string Exception = "exception";

            /// <summary>
            /// Тег для сообщений о событиях
            /// </summary>
            public const string Event = "event";
        }
    }
}