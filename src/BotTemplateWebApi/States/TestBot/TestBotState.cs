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
    private TelegramBotClient _botClient;
    
    public TestBotState(IServiceProvider serviceProvider)
    {
        ITelegramBotClient _botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Hello world");
        return Ok();
    }
}