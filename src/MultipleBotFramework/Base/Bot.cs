using Telegram.Bot;

namespace MultipleBotFramework.Base
{
    /// <summary>
    /// базовый класс бота 
    /// </summary>
    public class Bot
    {
        public TelegramBotClient ApiClient { get; set; }
        
        public Bot(TelegramBotClient apiClient)
        {
            ApiClient = apiClient;
        }
    }
}