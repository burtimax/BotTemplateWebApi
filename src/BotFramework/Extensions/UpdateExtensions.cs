using System;
using BotFramework.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Extensions
{
    public static class UpdateExtensions
    {
        
        /// <summary>
        /// Получить объект пользователя из Update для разных типов запросов.
        /// </summary>
        /// <param name="update">Объект запроса от бота.</param>
        /// <returns></returns>
        public static Chat? GetChat(this Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => update.Message.Chat,
                UpdateType.CallbackQuery => update.CallbackQuery.Message.Chat,
                UpdateType.ChannelPost => update.ChannelPost.Chat,
                UpdateType.ChatMember => update.ChatMember.Chat,
                UpdateType.EditedMessage => update.EditedMessage.Chat,
                UpdateType.ChatJoinRequest => update.ChatJoinRequest.Chat,
                UpdateType.MyChatMember => update.MyChatMember.Chat,
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
            return update.Type switch
            {
                UpdateType.Message => update.Message,
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
                UpdateType.Unknown => throw new UnknownUpdateTypeException()
            };
        }
        
        /// <summary>
        /// Получить объект пользователя <see cref="User"/> из Update для разных типов запросов.
        /// </summary>
        /// <param name="update">Объект запроса от бота.</param>
        /// <returns>User object</returns>
        public static User GetUser(this Update update)
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    return update.Message.From;
                    break;

                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.From;
                    break;

                case UpdateType.ChosenInlineResult:
                    return update.ChosenInlineResult.From;
                    break;

                case UpdateType.ChannelPost:
                    return update.ChannelPost.From;
                    break;

                case UpdateType.EditedChannelPost:
                    return update.EditedChannelPost.From;
                    break;

                case UpdateType.EditedMessage:
                    return update.EditedMessage.From;
                    break;

                case UpdateType.InlineQuery:
                    return update.InlineQuery.From;
                    break;

                case UpdateType.PollAnswer:
                    return update.PollAnswer.User;
                    break;

                case UpdateType.PreCheckoutQuery:
                    return update.PreCheckoutQuery.From;
                    break;

                case UpdateType.ShippingQuery:
                    return update.ShippingQuery.From;
                    break;
                default:
                    throw new NotImplementedException($"Not implemented [{nameof(GetUser)}] for update type [{update.Type}]");
            }

            return null;
        }
    }
}