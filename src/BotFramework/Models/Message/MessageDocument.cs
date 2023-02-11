using System.Threading.Tasks;


namespace BotFramework.Models.Message
{
    /// <summary>
    /// Документ
    /// </summary>
    public class MessageDocument : MessageData
    {
        public string FileName { get; set; }
        public string Caption { get; set; }

        /// <summary>
        /// Асинхронный фабричный метод
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns>Новый экземпляр</returns>
        public static Task<MessageDocument> GetNewAsync(FilePath filePath)
        {
            return MessageData.GetNewInstanceAsync<MessageDocument>(filePath);
        }

        public MessageDocument(FileData file, string caption = null)
        {
            this.File = file;
            this.Caption = caption;
        }

        public MessageDocument()
        {

        }
    }
}
