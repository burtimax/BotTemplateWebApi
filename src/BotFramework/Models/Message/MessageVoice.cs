using System.Threading.Tasks;

namespace BotFramework.Models.Message
{
    /// <summary>
    /// Голосовое сообщение
    /// </summary>
    public class MessageVoice : Models.Message.MessageData
    {

        // ToDo посмотреть где используется, если надо - убрать
        // public MessageVoice(FileData file)
        // {
        //     this.File = file;
        // }

        public MessageVoice()
        {
        }

        /// <summary>
        /// Асинхронный фабричный метод
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns>Новый экземпляр</returns>
        public static Task<MessageVoice> GetNewAsync(FilePath filePath)
        {
            return MessageData.GetNewInstanceAsync<MessageVoice>(filePath);
        }
    }
}