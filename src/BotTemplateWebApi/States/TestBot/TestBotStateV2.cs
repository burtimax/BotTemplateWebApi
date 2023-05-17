using BotFramework.Attributes;
using BotFramework.Controllers;
using BotFramework.Filters;
using BotTemplateWebApi.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.States.TestBot;

[BotState("test", version: 2)]
public class TestBotStateV2 : BaseBotState
{
    private TelegramBotClient _botClient;
    
    public TestBotStateV2(IServiceProvider serviceProvider)
    {
        IBotSingleton botSingleton = serviceProvider.GetRequiredService<IBotSingleton>();
        _botClient = botSingleton.GetInstance().ApiClient;
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Hello world V2");
        return Ok();
    }
}