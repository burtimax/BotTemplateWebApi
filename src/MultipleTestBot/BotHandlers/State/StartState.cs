using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Extensions.ITelegramApiClient;
using MultipleBotFramework.Utils.Keyboard;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleTestBot.BotHandlers.State;

[BotHandler(stateName:Name, version: 2.0f)]
public class StartState : BaseMultipleTestBotState
{
    public const string Name = "StartState";
    
    public StartState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        await Answer("Hello world");
    }
}