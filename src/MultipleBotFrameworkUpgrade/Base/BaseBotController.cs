using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.BotAPI.GettingUpdates;


namespace MultipleBotFrameworkUpgrade.Base;

[Controller]
public abstract class BaseBotController : ControllerBase
{
    public BaseBotController()
    {
    }

    public abstract Task<IActionResult> HandleBotRequest(long botId, Update update);
}