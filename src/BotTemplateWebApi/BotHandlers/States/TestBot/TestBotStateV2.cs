using BotFramework.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.States.TestBot;

[BotState("StartState", version: 2)]
public class TestBotStateV2 : BotState
{
    public TestBotStateV2(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        int t = 0;
        int v = 100 / t;
        
        var r = new Random(DateTime.Now.Millisecond);
        string message = r.Next() % 2 == 0 ? R.Test.Introduction : R.Test.Goodbye;
        await BotClient.SendTextMessageAsync(Chat.ChatId, message);
        return Ok();
    }
}