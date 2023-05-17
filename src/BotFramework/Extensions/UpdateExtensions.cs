using System;
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
        public static Chat GetChat(this Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    return update.Message.Chat;
                    break;

                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.Message.Chat;
                    break;
            }

            return null;
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