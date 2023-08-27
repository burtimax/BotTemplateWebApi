using BotFramework.Base;
using BotTemplateWebApi.Resources;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.States;

public class BotState : BaseBotState
{
    protected BotResources R;
    
    public BotState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
    }

    public override Task HandleBotRequest(Update update)
    {
        return null;
    }
}