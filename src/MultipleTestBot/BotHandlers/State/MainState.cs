// using BotFramework;
// 
// using BotFramework.Db.Entity;
// 
// using BotFramework.Services;
// 
// using MultipleTestBot.Db.Entities;
// using MultipleTestBot.Resources;
// using MultipleTestBot.Services;
// using Telegram.Bot;
// using Telegram.Bot.Types;
// using Telegram.Bot.Types.Enums;
// using Telegram.Bot.Types.ReplyMarkups;
//
// namespace MultipleTestBot.BotHandlers.State;
//
// [BotState(stateName:Name, version: 2.0f)]
// public class MainState : BaseMultipleTestBotState
// {
//     public const string Name = "MainState";
//     
//     private readonly ISavedMessageService _savedMessageService;
//     private readonly PublicMessageService _publicMessageService;
//     private readonly MessageViewService _messageViewService;
//     
//     
//     public MainState(IServiceProvider serviceProvider) : base(serviceProvider)
//     {
//         _savedMessageService = serviceProvider.GetRequiredService<ISavedMessageService>();
//         _publicMessageService = serviceProvider.GetRequiredService<PublicMessageService>();
//         _messageViewService = serviceProvider.GetRequiredService<MessageViewService>();
//     }
//
//     public override async Task HandleBotRequest(Update update)
//     {
//         if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message)
//         {
//             return;
//         }
//
//         // Не отвечаем на неподдерживаемые типы сообщений.
//         if (GetSupportedMessageTypes()
//                 .Contains(update.Message.Type) == false)
//         {
//             return;
//         }
//
//         if (update.Message.Type == MessageType.Text
//             && update.Message.Text == R.ButtonNext)
//         {
//             // Отправить случайное сообщение.
//             await SendRandomSavedMessage();
//             return;
//         }
//         
//         if (update.Message.Type == MessageType.Text)
//         {
//             await Answer(R.SendMeNoTextMessage);
//             return;
//         }
//         
//         // Сохраняем сообщение пользователя
//         await SaveMessage(update.Message!);
//         await SendRandomSavedMessage();
//
//         // if (update.Message.ReplyToMessage != null)
//         // {
//         //     Message repToMessage = update.Message.ReplyToMessage;
//         //     PublicMessageEntity? messageReply =
//         //         await _publicMessageService.GetPublicMessage(repToMessage.MessageId, repToMessage.MessageThreadId);
//         //
//         //     if (messageReply is not null)
//         //     {
//         //         await SendNotificationToUserAboutReply(messageReply);
//         //     }
//         // } 
//         return;
//     }
//
//     private async Task SendNotificationToUserAboutReply(PublicMessageEntity publicMessage)
//     {
//         ChatId chatId = new (publicMessage.SenderChatId);
//         await BotClient.SendSavedMessage(chatId, BotDbContext, publicMessage.SavedMessageId);
//         await BotClient.SendTextMessageAsync(chatId, R.ReplyForYourMessage);
//     }
//     
//     /// <summary>
//     /// Отправить случайное сообщение.
//     /// </summary>
//     public async Task SendRandomSavedMessage()
//     {
//         var next = await _publicMessageService.GetNext(Chat.ChatId);
//
//         if (next is null)
//         {
//             await Answer("Нет еще сообщений");
//             return;
//         }
//
//         var view = await _messageViewService.SaveView(Chat.ChatId.ToString(), next.Id);
//         await BotClient.SendSavedMessage(chatId: Chat.ChatId, BotDbContext, next.SavedMessageId);
//     }
//
//     public async Task<PublicMessageEntity> SaveMessage(Message message)
//     {
//         BotSavedMessage savedMessage = await _savedMessageService.SaveMessageFromUpdate(Chat, User, message);
//
//         var newMes = new PublicMessageEntity()
//         {
//             Id = savedMessage.Id,
//             SavedMessageId = savedMessage.Id,
//             TelegramMessageId = message.MessageId,
//             TelegramMessageThreadId = message.MessageThreadId,
//             Approved = true,
//             SenderChatId = Chat.ChatId.ToString(),
//         };
//         Db.PublicMessages.Add(newMes);
//         await Db.SaveChangesAsync();
//
//         await SendModeratorToApprove(newMes);
//         
//         return newMes;
//     }
//
//     private async Task SendModeratorToApprove(PublicMessageEntity mes)
//     {
//         var approveDeclineInline = new MarkupBuilder<InlineKeyboardMarkup>();
//         approveDeclineInline.NewRow()
//             .Add(R.ButtonApprove, BotResources.ButtonApproveCallbackKey + mes.SavedMessageId)
//             .Add(R.ButtonDecline, BotResources.ButtonDeclineCallbackKey + mes.SavedMessageId);
//         
//         await BotHelper.ExecuteFor(BotDbContext, BotConstants.BaseBotClaims.BotUserBlock, async tuple =>
//         {
//             await BotClient.SendSavedMessage(tuple.chat.ChatId, BotDbContext, mes.SavedMessageId);
//             await BotClient.SendTextMessageAsync(tuple.chat.ChatId, string.Format(R.NeedApprove, mes.SavedMessageId.ToString()), replyMarkup: approveDeclineInline.Build());
//         });
//     }
//     
//     private List<MessageType> GetSupportedMessageTypes() => new List<MessageType>()
//     {
//         MessageType.Animation,
//         MessageType.Audio,
//         MessageType.Text,
//         MessageType.Photo,
//         MessageType.Voice,
//         MessageType.Sticker,
//         MessageType.Video,
//         MessageType.VideoNote,
//         MessageType.Document,
//     };
// }