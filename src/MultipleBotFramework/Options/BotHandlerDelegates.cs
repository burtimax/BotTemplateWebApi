using System.Threading.Tasks;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Options;

public class BotHandlerDelegates
{
    /// <summary>
    /// Делегат для обработки Poll запроса бота.
    /// </summary>
    public delegate Task BotPollHandler(Poll poll);
    
    /// <summary>
    /// Делегат для обработки удаления бота у пользователя.
    /// </summary>
    public delegate Task BotDeleteHandler(Update update);
}