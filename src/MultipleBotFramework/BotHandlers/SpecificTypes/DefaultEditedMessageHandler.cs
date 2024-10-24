// using System;
// using System.Threading.Tasks;
// using MultipleBotFramework.Base;
// using MultipleBotFramework.Dispatcher.HandlerResolvers;
// using MultipleBotFramework.Enums;
// using Telegram.BotAPI.AvailableMethods;
// using Telegram.BotAPI.GettingUpdates;
//
// namespace MultipleBotFramework.BotHandlers.SpecificTypes;
//
// [BotHandler(updateTypes:new [] { UpdateType.Message }, version: 0)]
// public class DefaultEditedMessageHandler : BaseBotHandler
// {
//     public DefaultEditedMessageHandler(IServiceProvider serviceProvider) : base(serviceProvider)
//     {
//         
//     }
//
//     public override async Task HandleBotRequest(Update update)
//     {
//         return;
//     }
// }