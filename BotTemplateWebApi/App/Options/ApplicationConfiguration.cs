namespace BotTemplateWebApi.App.Options;

public class ApplicationConfiguration
{
    /// <summary>
    /// Конфигурации бота.
    /// </summary>
    public BotConfiguration Bot { get; set; }


    public class BotConfiguration
    {
        /// <summary>
        /// Токен бота из Telegram.org.
        /// </summary>
        public string TelegramToken { get; set; }
        
        /// <summary>
        /// URL адрес, на который будут отправляться запросы к боту для обработки.
        /// </summary>
        public string Webhook { get; set; }
    }
}