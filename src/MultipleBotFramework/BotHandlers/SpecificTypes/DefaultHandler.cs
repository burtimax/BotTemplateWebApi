using System;
using System.Threading.Tasks;
using MultipleBotFramework.Base;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.SpecificTypes;

[BotHandler(version: 0)]
public class DefaultHandler : BaseBotHandler
{
    public DefaultHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }

    public override async Task HandleBotRequest(Update update)
    {
#if DEBUG
        if (Chat is not null && Chat.ChatId != default)
        {
            try
            {
                await BotClient.SendMessageAsync(Chat.ChatId, $"DEFAULT [{update.Type().ToString()}]");
            }
            catch (Exception e)
            {
                
            }
        }
#endif
        
    }
}