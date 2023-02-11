using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Models.Message
{
    /// <summary>
    /// Аудиофайл
    /// </summary>
    public class MessageAudio : Models.Message.MessageData
    {
        public string Title { get; set; }
        public InputMedia Thumb { get; set; }

        public MessageAudio(FileData file)
        {
            this.File = file;
        }

        public MessageAudio()
        {
        }

        /// <summary>
        /// Асинхронный фабричный метод
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns>Новый экземпляр</returns>
        public static Task<MessageAudio> GetNewAsync(FilePath filePath)
        {
            return MessageData.GetNewInstanceAsync<MessageAudio>(filePath);
            
        }
    }
}