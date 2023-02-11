using System.Threading.Tasks;

namespace BotFramework.Models.Message
{
    /// <summary>
    /// Изображение
    /// </summary>
    public class MessagePicture : MessageData
    {
        /// <summary>
        /// Асинхронный фабричный метод
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns>Новый экземпляр</returns>
        public static Task<MessagePicture> GetNewAsync(FilePath filePath)
        {
            return MessageData.GetNewInstanceAsync<MessagePicture>(filePath);
        }

        public MessagePicture(FileData file, string caption = null)
        {
            this.File = file;
            this.Caption = caption;
        }

        public MessagePicture() : base()
        {

        }
    }
}
