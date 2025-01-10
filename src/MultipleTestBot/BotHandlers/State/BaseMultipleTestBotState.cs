


using Microsoft.Extensions.Options;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Utils;
using MultipleTestBot.Resources;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;


namespace MultipleTestBot.BotHandlers.State;

public class BaseMultipleTestBotState : BaseBotHandler
{
    protected readonly BotResources R;
    
    public BaseMultipleTestBotState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
    }
}