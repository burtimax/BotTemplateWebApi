using System;
using System.IO;
using System.Threading.Tasks;
using BotFramework.Enums;
using BotFramework.Models;
using BotFramework.Models.Message;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using File = System.IO.File;

namespace BotFramework.Other
{
    public class BotMediaHelper
    {
        /// <summary>
        /// Скачать и сохранить файл из Telegram серверов в директорию.
        /// </summary>
        /// <param name="botClient">API клиент.</param>
        /// <param name="file">Данные о файле.</param>
        /// <param name="fp">Полный путь к файлу.</param>
        public static async Task DownloadAndSaveTelegramFileAsync(ITelegramBotClient botClient, FileBase file, FilePath fp)
        {
            using (FileStream fs = new FileStream(fp, FileMode.Create))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    await botClient.GetInfoAndDownloadFileAsync(file.FileId, ms);
                    ms.Position = 0;
                    await fs.WriteAsync(ms.GetBuffer());
                }
            }
        }

        /// <summary>
        /// Получить файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns></returns>
        public static async Task<InputOnlineFile> GetFileByPathAsync(string filePath)
        {
            MemoryStream ms = new(await File.ReadAllBytesAsync(filePath));
            ms.Position = 0;
            return new InputOnlineFile(ms);
        }

        /// <summary>
        /// Скачать файл из серверов Telegram по ИД.
        /// </summary>
        /// <param name="botClient">API клиент.</param>
        /// <param name="fileId">ИД файла.</param>
        /// <returns>Данные по файлу.</returns>
        public static async Task<(Telegram.Bot.Types.File fileInfo, MemoryStream fileData)> GetFileFromTelegramAsync(ITelegramBotClient botClient, string fileId)
        {
            MemoryStream ms = new();
            Telegram.Bot.Types.File res = await botClient.GetInfoAndDownloadFileAsync(fileId, ms);
            ms.Position = 0;
            return (res, ms);
        }
        
        public static async Task<MessagePicture> GetPhotoAsync(ITelegramBotClient bot, Message mes, PhotoQuality quality = PhotoQuality.High)
        {
            if (bot == null ||
                mes == null ||
                mes.Type != MessageType.Photo) return null;

            int qualityIndex = (int) Math.Round(((int)quality) / ((double)PhotoQuality.High) * mes.Photo.Length-1);
            string fileId = null;
            fileId = mes.Photo[qualityIndex].FileId;
            MessagePicture picture = new MessagePicture();
            picture.File = await GetFile(bot, mes, fileId) ;
            return picture;
        }


        public static async Task<MessageAudio> GetAudioAsync(ITelegramBotClient bot, Message mes)
        {
            if (bot == null ||
                mes == null ||
                mes.Type != MessageType.Audio) return null;

            MessageAudio audio = new MessageAudio();
            audio.File = await GetFile(bot, mes, mes.Audio.FileId);
            return audio;
        }

        public static async Task<MessageVoice> GetVoiceAsync(ITelegramBotClient bot, Message mes)
        {
            if (bot == null || 
                mes == null ||
                mes.Type != MessageType.Voice) return null;

            MessageVoice voice = new MessageVoice();
            var fileId = mes.Voice.FileId;
            voice.File = await GetFile(bot, mes, fileId);
            return voice;
        }

        private static async Task<FileData> GetFile(ITelegramBotClient bot, Message mes, string fileId)
        {
            if (bot == null || 
                mes == null ||
                string.IsNullOrEmpty(fileId)) return null ;

            FileData fileData = new FileData();
            fileData.Stream = new MemoryStream();
            fileData.Info = await bot.GetInfoAndDownloadFileAsync(fileId, fileData.Stream);
            return fileData;
        }
    }
}
