using BotFramework.Attributes;
using BotFramework.Base;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.BotHandlers.Commands.Hello;

[BotCommand("/hello", version:2.1)]
public class HelloCommandV2: BaseBotCommand
{
    public HelloCommandV2(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        await BotClient.SendTextMessageAsync(Chat.ChatId, "Hello V2");
    }
}