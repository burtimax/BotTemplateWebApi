using System;
using System.IO;
using System.Threading.Tasks;
using BotFramework.Enums;
using BotFramework.Exceptions;
using BotFramework.Models.Message;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Extensions
{
    public static class MessageDataExtensions
    {
        /// <summary>
        /// Получить изображение из сообщения
        /// </summary>
        /// <param name="bot">клиент telegram bot api</param>
        /// <param name="mes">сообщение</param>
        /// <param name="quality">тип качества изображения</param>
        /// <returns>Загружает изображение и возвращает объект</returns>
        public static async Task<MessagePicture> GetPhotoAsync(this TelegramBotClient bot, Message mes, PhotoQuality quality = PhotoQuality.High)
        {
            if (mes == null)
            {
                throw new NullReferenceException();
            }
                
            if(mes.Type != MessageType.Photo)
            {
                throw new UnexpectedMessageType();
            }
            
            //ToDo проверить получение фотографий различного качества
            //int qualityIndex = (int) Math.Round(((int)quality) / ((double)PhotoQuality.High) * mes.Photo.Length-1);//Рабочий вариант, но мне не нравится это стращное выражение
            string fileId = mes.Photo![(int)quality].FileId;
            MessagePicture picture = new MessagePicture();
            picture.File = await GetFile(bot, mes, fileId) ;
            return picture;
        }

        /// <summary>
        /// Получить аудио файл из сообщения
        /// </summary>
        /// <param name="bot">клиент telegram bot api</param>
        /// <param name="mes">сообщение</param>
        /// <returns>Загружает аудио и возвращает объект</returns>
        public static async Task<MessageAudio> GetAudioAsync(this TelegramBotClient bot, Message mes)
        {
            if (mes == null)
            {
                throw new NullReferenceException();
            }
                
            if(mes.Type != MessageType.Audio)
            {
                throw new UnexpectedMessageType();
            }

            MessageAudio audio = new MessageAudio();
            audio.File = await GetFile(bot, mes, mes!.Audio!.FileId);
            return audio;
        }

        /// <summary>
        /// Получить голосовой файл из сообщения
        /// </summary>
        /// <param name="bot">клиент telegram bot api</param>
        /// <param name="mes">сообщение</param>
        /// <returns>Загружает голосовое сообщение и возвращает объект</returns>
        public static async Task<MessageVoice> GetVoiceAsync(this TelegramBotClient bot, Message mes)
        {
            if (mes == null)
            {
                throw new NullReferenceException();
            }
                
            if(mes.Type != MessageType.Voice)
            {
                throw new UnexpectedMessageType();
            }

            MessageVoice voice = new MessageVoice();
            voice.File = await GetFile(bot, mes, mes!.Voice!.FileId);
            return voice;
        }

        /// <summary>
        /// Загрузить файл из Telegram по ИД
        /// </summary>
        /// <param name="bot">клиент telegram bot api</param>
        /// <param name="mes">сообщение</param>
        /// <param name="fileId">ИД файла</param>
        /// <returns>Загружает файл и возвращает его</returns>
        private static async Task<FileData> GetFile(this TelegramBotClient bot, Message mes, string fileId)
        {
            if (mes == null || 
                string.IsNullOrEmpty(fileId))
            {
                throw new NullReferenceException();
            }

            FileData fileData = new();
            fileData.Stream = new MemoryStream();
            fileData.Info = await bot.GetInfoAndDownloadFileAsync(fileId, fileData.Stream);
            return fileData;
        }
    }
}