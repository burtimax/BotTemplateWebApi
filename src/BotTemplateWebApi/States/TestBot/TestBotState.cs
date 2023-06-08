using BotFramework.Attributes;
using BotFramework.Controllers;
using BotFramework.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.States.TestBot;

[BotState("test")]
public class TestBotState : BaseBotState
{

    public TestBotState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        await BotClient.SendTextMessageAsync(update.Message.Chat.Id, "Hello world");
        return Ok();
    }
}