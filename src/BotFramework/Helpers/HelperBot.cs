using System;
using System.IO;
using System.Threading.Tasks;
using BotFramework.Enums;
using BotFramework.Models.Message;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Helpers
{
    public class HelperBot
    {
        public static async Task<MessagePicture> GetPhotoAsync(TelegramBotClient bot, Message mes, PhotoQuality quality = PhotoQuality.High)
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


        public static async Task<MessageAudio> GetAudioAsync(TelegramBotClient bot, Message mes)
        {
            if (bot == null ||
                mes == null ||
                mes.Type != MessageType.Audio) return null;

            MessageAudio audio = new MessageAudio();
            audio.File = await GetFile(bot, mes, mes.Audio.FileId);
            return audio;
        }

        public static async Task<MessageVoice> GetVoiceAsync(TelegramBotClient bot, Message mes)
        {
            if (bot == null || 
                mes == null ||
                mes.Type != MessageType.Voice) return null;

            MessageVoice voice = new MessageVoice();
            var fileId = mes.Voice.FileId;
            voice.File = await GetFile(bot, mes, fileId);
            return voice;
        }

        private static async Task<FileData> GetFile(TelegramBotClient bot, Message mes, string fileId)
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
