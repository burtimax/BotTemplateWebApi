using System.Threading.Tasks;
using Telegram.Bot;

namespace BotFramework.Interfaces
{
    /// <summary>
    /// Обработка сообщений или событий бота 
    /// </summary>
    interface IUpdateHandler
    {
        /// <summary>
        /// Телеграм Bot API клиент
        /// </summary>
        public TelegramBotClient BotClient { get; set; }

        /// <summary>
        /// Обработчик обновлений (сообщения/события)
        /// </summary>
        Task HandleUpdateAsync();
    }
}
