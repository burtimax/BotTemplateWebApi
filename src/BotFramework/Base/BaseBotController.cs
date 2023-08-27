using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BotFramework.Base;

[Controller]
public abstract class BaseBotController : ControllerBase
{
    public BaseBotController()
    {
    }

    public abstract Task<IActionResult> HandleBotRequest(Update update);
}