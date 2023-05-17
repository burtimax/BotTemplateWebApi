using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BotFramework.Controllers;

public abstract class BaseBotState : ControllerBase
{
    public BaseBotState()
    {
    }

    public abstract Task<IActionResult> HandleBotRequest(Update update);
}