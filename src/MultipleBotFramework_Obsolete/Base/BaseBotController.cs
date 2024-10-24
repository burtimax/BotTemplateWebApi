using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace MultipleBotFramework_Obsolete.Base;

[Controller]
public abstract class BaseBotController : ControllerBase
{
    public BaseBotController()
    {
    }

    public abstract Task<IActionResult> HandleBotRequest(long botId, Update update);
}