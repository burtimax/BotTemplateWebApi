using System;
using System.Threading.Tasks;
using MultipleBotFramework.Base;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.SpecificTypes;

[BotHandler(updateTypes:new [] { UpdateType.CallbackQuery }, version:0)]
public class DefaultCallbackQueryHandler : BaseBotHandler
{
    public DefaultCallbackQueryHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }

    public override async Task HandleBotRequest(Update update)
    {
        var callback = update.CallbackQuery!;
#if DEBUG
        try
        {
            await BotClient.AnswerCallbackQueryAsync(callback.Id);
            await BotClient.SendMessageAsync(Chat.ChatId, $"DEFAULT [{update.Type().ToString()}]");
        }
        catch (Exception e) { }
#endif
    }
}