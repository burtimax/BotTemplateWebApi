// using Microsoft.Extensions.Options;
// using MultipleBotFramework.Attributes;
// using MultipleBotFramework.Base;
// using MultipleBotFramework.Dispatcher.HandlerResolvers;
// using MultipleBotFramework.Extensions;
// using MultipleTestBot.Resources;
// using Telegram.BotAPI.AvailableMethods;
// using Telegram.BotAPI.GettingUpdates;
//
//
// namespace MultipleTestBot.BotHandlers.UpdateType;
//
// [BotHandler(updateTypes: new []{ MultipleBotFramework.Enums.UpdateType.CallbackQuery })]
// public class CallbackUpdateHandler : BaseBotHandler
// {
//     protected readonly BotResources R;
//     
//     public CallbackUpdateHandler(IServiceProvider serviceProvider) : base(serviceProvider)
//     {
//         R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
//     }
//
//     public override async Task HandleBotRequest(Update update)
//     {
//         if (update.Type() != MultipleBotFramework.Enums.UpdateType.CallbackQuery) return;
//
//         
//         var callback = update.CallbackQuery;
//
//         await BotClient.AnswerCallbackQueryAsync(callback.Id);
//         await BotClient.SendMessageAsync(Chat.ChatId, "Callback handler");
//     }
// }