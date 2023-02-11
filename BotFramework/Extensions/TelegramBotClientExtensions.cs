using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Enums;
using BotFramework.Exceptions;
using BotFramework.Models;
using BotFramework.Models.Message;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotFramework.Extensions
{
    public static class TelegramBotClientExtensions
    {
        public static Task SendOutboxMessageAsync(this TelegramBotClient bot, ChatId chatId, OutboxMessage message)
        {
            return SendOutboxMessageToChat(bot, chatId, message);
        }
        public static Task SendOutboxMessageAsync(this TelegramBotClient bot, long id, OutboxMessage message)
        {
            return SendOutboxMessageToChat(bot, new ChatId(id), message);
        }
        public static Task SendOutboxMessageAsync(this TelegramBotClient bot, string username, OutboxMessage message)
        {
            return SendOutboxMessageToChat(bot, new ChatId(username), message);
        }

        private static async Task SendOutboxMessageToChat(this TelegramBotClient bot, ChatId chatId, OutboxMessage message)
        {
            
            if (bot == null ||
                chatId == null)
            {
                throw new ArgumentNullException();
            }

            switch (message.Type)
            {
                //Send Text
                case OutboxMessageType.Text:
                    await bot.SendTextMessageAsync(
                        chatId: chatId, 
                        text: (string) message.Data, 
                        replyMarkup:message.ReplyMarkup,
                        parseMode: message.ParseMode,
                        replyToMessageId: message.ReplyToMessageId);
                    break;

                //Send MessagePhoto Entity
                case OutboxMessageType.Photo:
                    MessagePicture picture = (MessagePicture) message.Data;
                    var file = new InputOnlineFile(picture.File.Stream);
                    await bot.SendPhotoAsync(
                        chatId: chatId,
                        photo: file,
                        caption: picture.Caption,
                        replyToMessageId: message.ReplyToMessageId,
                        parseMode: message.ParseMode,
                        replyMarkup: message.ReplyMarkup);
                    break;
                //Send MessageAudio Entity
                case OutboxMessageType.Audio:
                    MessageAudio audio = (MessageAudio)message.Data;
                    await bot.SendAudioAsync(
                        chatId: chatId, 
                        audio: new InputOnlineFile(audio.File.Stream),
                        caption: audio.Caption,
                        replyMarkup: message.ReplyMarkup,
                        replyToMessageId: message.ReplyToMessageId,
                        thumb: audio.Thumb,
                        title: audio.Title,
                        parseMode: message.ParseMode);
                    break;

                //Send MessageVoice Entity
                case OutboxMessageType.Voice:
                    MessageVoice voice = (MessageVoice)message.Data; 
                    await bot.SendVoiceAsync(
                        chatId: chatId, 
                        voice: new InputOnlineFile(voice.File.Stream), 
                        replyMarkup: message.ReplyMarkup,
                        replyToMessageId: message.ReplyToMessageId,
                        caption: voice.Caption,
                        parseMode: message.ParseMode);
                    break;

                case OutboxMessageType.MediaGroup:
                    MessageMediaGroup media = (MessageMediaGroup) message.Data;
                    List<IAlbumInputMedia> mediaList = new List<IAlbumInputMedia>();
                    
                    for (var i = 0; i < media.Files.Count(); i++)
                    {
                        var inputMedia = new InputMediaPhoto(new InputMedia(media.Files[i].Stream,
                            $"file{new Random((int)DateTime.Now.Ticks).Next(0, Int32.MaxValue).ToString()}.jpg"));
                        if (i == 0 && string.IsNullOrEmpty(media.Caption) == false)
                        {
                            inputMedia.Caption = media.Caption;
                        }
                        mediaList.Add(inputMedia);
                    }

                    await bot.SendMediaGroupAsync(chatId, mediaList);
                    break;
                //Send Document Type Message
                case OutboxMessageType.Document:
                    MessageDocument document = (MessageDocument)message.Data;
                    InputOnlineFile iof = new InputOnlineFile(document.File.Stream);
                    if(String.IsNullOrEmpty(document.FileName) == false)
                    {
                        iof.FileName = document.FileName;
                    }

                    await bot.SendDocumentAsync(chatId,
                        document: iof,
                        caption: document.Caption,
                        parseMode: message.ParseMode,
                        replyMarkup: message.ReplyMarkup,
                        replyToMessageId: message.ReplyToMessageId);
                    break;

                //ToDo other Types of message!
                default:
                    throw new UnsupportedMessageTypeException(nameof(message.Type));
                    break;
            }
            
            //Рекурсивно вызываем отправку вложенных элементов сообщения.
            foreach (var item in message.NestedElements)
            {
                await SendOutboxMessageToChat(bot, chatId, item);
            }
        }

        /// <summary>
        /// Получить ИД чата из обновления (Обрати внимание, ели чат приватный, то UserId == ChatId, иначе не так)
        /// </summary>
        /// <param name="bot">Клиент Telegram API bot</param>
        /// <param name="update">Сообщение или событие из Telegram для бота</param>
        /// <returns>Идентификатор чата</returns>
        public static ChatId GetChatId(this TelegramBotClient bot, Update update)
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    return update.Message.Chat.Id;
                    break;

                case UpdateType.CallbackQuery:
                    return update.CallbackQuery.From.Id;
                    break;

                case UpdateType.ChosenInlineResult:
                    return update.ChosenInlineResult.From.Id;
                    break;

                case UpdateType.ChannelPost:
                    return update.ChannelPost.Chat.Id;
                    break;

                case UpdateType.EditedChannelPost:
                    return update.EditedChannelPost.Chat.Id;
                    break;

                case UpdateType.EditedMessage:
                    return update.EditedMessage.Chat.Id;
                    break;

                case UpdateType.InlineQuery:
                    return update.InlineQuery.From.Id;
                    break;

                case UpdateType.PollAnswer:
                    return update.PollAnswer.User.Id;
                    break;

                case UpdateType.PreCheckoutQuery:
                    return update.PreCheckoutQuery.From.Id;
                    break;

                case UpdateType.ShippingQuery:
                    return update.ShippingQuery.From.Id;
                    break;
            }

            return null;
        }
    }
}
