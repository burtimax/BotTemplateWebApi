using BotFramework.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.States.TestBot;

[BotState("test")]
public class TestBotState : BotState
{

    public TestBotState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }

    public override async Task HandleBotRequest(Update update)
    {
        var r = new Random(DateTime.Now.Millisecond);
        string message = r.Next() % 2 == 0 ? R.Test.Introduction : R.Test.Farewell;
        await BotClient.SendTextMessageAsync(Chat.ChatId, message);
    }
}