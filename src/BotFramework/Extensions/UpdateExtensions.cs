﻿using System;
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
                UpdateType.InlineQuery => null,
                UpdateType.ShippingQuery => null,
                UpdateType.PollAnswer => null,
                UpdateType.ChosenInlineResult => null,
                UpdateType.EditedChannelPost => null,
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
        public static User? GetUser(this Update update)
        {
            return update.Type switch
            {
                UpdateType.Message => update.Message.From,
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
    }
}