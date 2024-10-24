using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleTestBot.BotHandlers.State;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleTestBot.BotHandlers.Command;

[BotHandler(command:"/start", version: 2.0f)]
public class StartCommand : BaseMultipleTestBotCommand
{
    public StartCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        await Answer("Start world!");
    }
}