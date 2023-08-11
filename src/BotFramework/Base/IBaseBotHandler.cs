using System.Threading.Tasks;
using BotFramework.Db.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Base;

public interface IBaseBotHandler
{
    public BotUser User { get; set; }
    public BotChat Chat { get; set; }
    public ITelegramBotClient BotClient { get; set; }
    Task HandleBotRequest(Update update);
}