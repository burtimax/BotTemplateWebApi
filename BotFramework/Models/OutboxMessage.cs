using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Enums;
using BotFramework.Models.Message;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Models
{
    /// <summary>
    /// Исходящее сообщение
    /// </summary>
    public class OutboxMessage
    {
        /// <summary>
        /// Тип сообщения (текст, докумень, картинка, голосовое и т.д.)
        /// </summary>
        public OutboxMessageType Type { get; private set; }
        
        /// <summary>
        /// Объект сообщения.
        /// </summary>
        public object Data { get; private set; }
        
        /// <summary>
        /// Вложенные сообщения (если нужно отправить последовательно несколько сообщений)
        /// </summary>
        public List<OutboxMessage>? NestedElements { get; private set; }
        
        /// <summary>
        /// Пользовательская клавиатура
        /// </summary>
        public IReplyMarkup ReplyMarkup { get; set; }
        
        /// <summary>
        /// Пометка, что сообщение является ответом на другое сообщение
        /// </summary>
        public int ReplyToMessageId { get; set; }

        /// <summary>
        /// Тип парсера сообщения
        /// </summary>
        public ParseMode ParseMode { get; set; } = ParseMode.Html;

        #region Constructors

    
        public OutboxMessage()
        {
            this.NestedElements = new List<OutboxMessage>();
        }

        private OutboxMessage(object data) : this()
        {
            this.Data = data;
        }

        public OutboxMessage(MessagePicture picture) : this((object) picture)
        {
            this.Type = OutboxMessageType.Photo;
        }

        public OutboxMessage(MessageMediaGroup mediaGroup) : this((object)mediaGroup)
        {
            this.Type = OutboxMessageType.MediaGroup;
        }

        public OutboxMessage(MessageDocument document) : this((object) document)
        {
            this.Type = OutboxMessageType.Document;
        }

        public OutboxMessage(MessageAudio Audio) : this((object) Audio)
        {
            this.Type = OutboxMessageType.Audio;
        }

        public OutboxMessage(MessageVoice Voice) : this((object)Voice)
        {
            this.Type = OutboxMessageType.Voice;
        }

        public OutboxMessage(string text) : this((object) text)
        {
            this.Type = OutboxMessageType.Text;
        }

        #endregion

        public OutboxMessage this[int index]
        {
            get
            {
                if (NestedElements.Count == 0) return null;

                if (index > NestedElements.Count || index<0)
                {
                    throw new Exception("Выход за границы списка!");
                }

                return this.NestedElements.ElementAt(index);
            }
            set
            {
                if (NestedElements == null || this.NestedElements.Count == 0)
                {
                    throw new Exception("Выход за границы списка!");
                }

                if (index > NestedElements.Count || index < 0)
                {
                    throw new Exception("Выход за границы списка!");
                }

                this.NestedElements[index] = value;
            }
        }

        public void AddMessageElement(OutboxMessage elem)
        {
            if (elem == null && elem.Data == null) return;
            
            if (NestedElements == null)
            {
                NestedElements = new List<OutboxMessage>();
            }

            this.NestedElements.Add(elem);
        }
        
        public void RemoveMessageElement(OutboxMessage elem)
        {
            if (NestedElements == null && NestedElements?.Count == 0)
            {
                return;
            }

            if (NestedElements.Contains(elem))
            {
                NestedElements.Remove(elem);
            }
        }
        
        public Task SendOutboxMessage(TelegramBotClient bot, ChatId chatId)
        {
            return SendOutboxMessageToChat(bot, chatId);
        }
        public Task SendOutboxMessage(TelegramBotClient bot, long id)
        {
            return SendOutboxMessageToChat(bot, new ChatId(id));
        }
        public Task SendOutboxMessage(TelegramBotClient bot, string username)
        {
            return SendOutboxMessageToChat(bot, new ChatId(username));
        }

        private async Task SendOutboxMessageToChat(TelegramBotClient bot, ChatId chatId)
        {

            if (bot == null ||
                ((chatId == null || chatId?.Identifier == 0) &&
                 string.IsNullOrEmpty(chatId?.Username)))
            {
                throw new ArgumentNullException();
            }

            switch (this.Type)
            {
                //Send Text
                case OutboxMessageType.Text:
                    await bot.SendTextMessageAsync(
                    chatId: chatId,
                    text: (string)this.Data,
                    replyMarkup: this.ReplyMarkup,
                    parseMode: this.ParseMode,
                    replyToMessageId: this.ReplyToMessageId);
                    break;

                //Send MessagePhoto Entity
                case OutboxMessageType.Photo:
                    MessagePicture picture = (MessagePicture)this.Data;
                    await bot.SendPhotoAsync(
                        chatId: chatId,
                        photo: new InputOnlineFile(picture.File.Stream),
                        caption: picture.Caption,
                        replyToMessageId: this.ReplyToMessageId,
                        parseMode: this.ParseMode);
                    break;
                //Send MessageAudio Entity
                case OutboxMessageType.Audio:
                    MessageAudio audio = (MessageAudio)this.Data;
                    await bot.SendAudioAsync(
                        chatId: chatId,
                        audio: new InputOnlineFile(audio.File.Stream),
                        caption: audio.Caption,
                        replyMarkup: this.ReplyMarkup,
                        replyToMessageId: this.ReplyToMessageId,
                        thumb: audio.Thumb,
                        title: audio.Title,
                        parseMode: this.ParseMode);
                    break;

                //Send MessageVoice Entity
                case OutboxMessageType.Voice:
                    MessageVoice voice = (MessageVoice)this.Data;
                    await bot.SendVoiceAsync(
                        chatId: chatId,
                        voice: new InputOnlineFile(voice.File.Stream),
                        replyMarkup: this.ReplyMarkup,
                        replyToMessageId: this.ReplyToMessageId,
                        caption: voice.Caption,
                        parseMode: this.ParseMode);
                    break;
                //ToDo other Types of message!
                default:
                    throw new Exception("Не поддерживаемый тип отправки сообщений");
                    break;
            }

            //Рекурсивно вызываем отправку вложенных элементов сообщения.
            foreach (var item in this.NestedElements)
            {
                await item.SendOutboxMessageToChat(bot, chatId);
            }
        }



    }
}