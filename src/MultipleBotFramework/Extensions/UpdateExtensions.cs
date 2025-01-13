using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Exceptions;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Extensions
{
    public static class UpdateExtensions
    {
        
        /// <summary>
        /// Получить объект пользователя из Update для разных типов запросов.
        /// </summary>
        /// <param name="update">Объект запроса от бота.</param>
        /// <returns></returns>
        public static UpdateType Type(this Update update)
        {
            var updateType = update switch
            {
                { Message: { } }            => UpdateType.Message,
                { EditedMessage: { } }      => UpdateType.EditedMessage,
                { InlineQuery: { } }        => UpdateType.InlineQuery,
                { ChosenInlineResult: { } } => UpdateType.ChosenInlineResult,
                { CallbackQuery: { } }      => UpdateType.CallbackQuery,
                { ChannelPost: { } }        => UpdateType.ChannelPost,
                { EditedChannelPost: { } }  => UpdateType.EditedChannelPost,
                { ShippingQuery: { } }      => UpdateType.ShippingQuery,
                { PreCheckoutQuery: { } }   => UpdateType.PreCheckoutQuery,
                { Poll: { } }               => UpdateType.Poll,
                { PollAnswer: { } }         => UpdateType.PollAnswer,
                { MyChatMember: { } }       => UpdateType.MyChatMember,
                { ChatMember: { } }         => UpdateType.ChatMember,
                { ChatJoinRequest: { } }    => UpdateType.ChatJoinRequest,
                { MessageReactionCount: { } }    => UpdateType.MessageReactionCount,
                _                           => UpdateType.Unknown
            };

            if (updateType is UpdateType.Message
                && update.Message!.Type() == MessageType.Text
                && update.Message!.Text!.StartsWith("/")) updateType = UpdateType.Command;

            return updateType;
        }
        
        /// <summary>
        /// Получить объект пользователя из Update для разных типов запросов.
        /// </summary>
        /// <param name="update">Объект запроса от бота.</param>
        /// <returns></returns>
        public static Chat? GetChat(this Update update)
        {
            return update.Type() switch
            {
                UpdateType.Message => update.Message.Chat,
                UpdateType.Command => update.Message.Chat,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat,
                UpdateType.ChannelPost => update.ChannelPost.Chat,
                UpdateType.ChatMember => update.ChatMember.Chat,
                UpdateType.EditedMessage => update.EditedMessage.Chat,
                UpdateType.ChatJoinRequest => update.ChatJoinRequest.Chat,
                UpdateType.MyChatMember => update.MyChatMember.Chat,
                UpdateType.InlineQuery => null,
                UpdateType.ShippingQuery => null,
                UpdateType.PollAnswer => update.PollAnswer.VoterChat,
                UpdateType.ChosenInlineResult => null,
                UpdateType.EditedChannelPost => update.EditedChannelPost.Chat,
                UpdateType.PreCheckoutQuery => null,
                UpdateType.Poll => null,
                UpdateType.Unknown => null,
                _ => null,
            };
        }

        /// <summary>
        /// Получить полезную нагрузку объекта запроса.
        /// </summary>
        /// <param name="update">Объект запроса бота.</param>
        /// <returns></returns>
        public static object GetPayload(this Update update)
        {
            return update.Type() switch
            {
                UpdateType.Message => update.Message,
                UpdateType.Command => update.Message,
                UpdateType.Poll => update.Poll,
                UpdateType.CallbackQuery => update.CallbackQuery,
                UpdateType.ChannelPost => update.ChannelPost,
                UpdateType.ChatMember => update.ChatMember,
                UpdateType.EditedMessage => update.EditedMessage,
                UpdateType.InlineQuery => update.InlineQuery,
                UpdateType.PollAnswer => update.PollAnswer,
                UpdateType.ShippingQuery => update.ShippingQuery,
                UpdateType.ChatJoinRequest => update.ChatJoinRequest,
                UpdateType.ChosenInlineResult => update.ChosenInlineResult,
                UpdateType.EditedChannelPost => update.EditedChannelPost,
                UpdateType.MyChatMember => update.MyChatMember,
                UpdateType.PreCheckoutQuery => update.PreCheckoutQuery,
                UpdateType.Unknown => null, //throw new UnknownUpdateTypeException()
            };
        }
        
        /// <summary>
        /// Получить объект пользователя <see cref="User"/> из Update для разных типов запросов.
        /// </summary>
        /// <param name="update">Объект запроса от бота.</param>
        /// <returns>User object</returns>
        public static User? GetUser(this Update update)
        {
            return update.Type() switch
            {
                UpdateType.Message => update.Message.From,
                UpdateType.Command => update.Message.From,
                UpdateType.CallbackQuery => update.CallbackQuery.From,
                UpdateType.ChannelPost => update.ChannelPost.From,
                UpdateType.ChatMember => update.ChatMember.From,
                UpdateType.EditedMessage => update.EditedMessage.From,
                UpdateType.InlineQuery => update.InlineQuery.From,
                UpdateType.ShippingQuery => update.ShippingQuery.From,
                UpdateType.PollAnswer => update.PollAnswer.User,
                UpdateType.ChatJoinRequest => update.ChatJoinRequest.From,
                UpdateType.ChosenInlineResult => update.ChosenInlineResult.From,
                UpdateType.EditedChannelPost => update.EditedChannelPost.From,
                UpdateType.MyChatMember => update.MyChatMember.From,
                UpdateType.PreCheckoutQuery => update.PreCheckoutQuery.From,
                UpdateType.Poll => null,
                UpdateType.Unknown => null,
                _ => null,
            };
        }

        public static bool TrySetContentToChatHistory(this Update u, ref BotChatHistoryEntity item)
        {
            item.JsonData = u.GetPayload()?.ToJson();
            item.MessageType = u.Type().ToString();

            ChatHistoryType? type = null;
            string content = $"#[{u.Type().ToString()}]\n";
            
            switch (u.Type())
            {
                case UpdateType.Message: return u.Message!.TrySetContentToChatHistory(ref item); break;
                case UpdateType.Command: return u.Message!.TrySetContentToChatHistory(ref item); break;
                case UpdateType.CallbackQuery:
                    type = ChatHistoryType.Callback;
                    content += u.CallbackQuery.Data;
                    break;
                case UpdateType.MyChatMember:
                    type = ChatHistoryType.MyChatMember;
                    if (u.MyChatMember.NewChatMember is ChatMemberBanned) content += "Пользователь заблокировал бота.";
                    else if (u.MyChatMember.NewChatMember is ChatMemberRestricted) content += "Изменены разрешения пользователя.";
                    else if (u.MyChatMember.NewChatMember is ChatMemberMember) content += "Пользователь разблокировал бота.";
                    else return false;
                    break;
                case UpdateType.EditedMessage:
                    type = ChatHistoryType.EditedMessage;
                    content += u.EditedMessage.MessageId;
                    break;
                // case UpdateType.ChatMember: update.ChatMember.From, break;
                // case UpdateType.ChannelPost: update.ChannelPost.From, break;
                // case UpdateType.InlineQuery: update.InlineQuery.From, break;
                // case UpdateType.ShippingQuery: update.ShippingQuery.From, break;
                // case UpdateType.PollAnswer: update.PollAnswer.User, break;
                // case UpdateType.ChatJoinRequest: update.ChatJoinRequest.From, break;
                // case UpdateType.ChosenInlineResult: update.ChosenInlineResult.From, break;
                // case UpdateType.EditedChannelPost: update.EditedChannelPost.From, break;
                // case UpdateType.PreCheckoutQuery: update.PreCheckoutQuery.From, break;
                // case UpdateType.Poll: null, break;
                // case UpdateType.Unknown: null, break;
                // _ => null, break;
                default: return false;
            }

            item.Text = content;
            if(type is not null) item.Type = type.Value;
            return true;
        }
    }
}