using BotFramework.Attributes;
using BotFramework.Base;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.BotHandlers.Commands.Hello;

[BotCommand("/hello")]
public class HelloCommand: BaseBotCommand
{
    public HelloCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        await BotClient.SendTextMessageAsync(Chat.ChatId, "Hello");
    }
}