namespace MultipleBotFramework
{
    public partial class BotConstants
    {
        /// <summary>
        /// Класс, содержащий ограничения Telegram API и другие системные ограничения.
        /// Используется для валидации данных и предотвращения ошибок при работе с API Telegram.
        /// </summary>
        public class Constraints
        {
            /// <summary>
            /// Максимальная длина текстового сообщения в Telegram.
            /// Превышение этого лимита приведет к ошибке при отправке сообщения.
            /// </summary>
            public const int MaxMessageLength = 4096;
            
            /// <summary>
            /// Максимальная длина подписи (caption) к документу в Telegram.
            /// Используется при отправке файлов, фотографий и других медиа-файлов.
            /// </summary>
            public const int MaxDocumentCaption = 1024;
        }
    }
}