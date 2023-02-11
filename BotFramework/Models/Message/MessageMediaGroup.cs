using System.Collections.Generic;

namespace BotFramework.Models.Message
{
    /// <summary>
    /// Группа медиа файлов
    /// </summary>
    public class MessageMediaGroup
    {
        /// <summary>
        /// Список файлов
        /// </summary>
        public List<FileData> Files { get; set; }
        
        /// <summary>
        /// Сообщение к группе медиа файлов
        /// </summary>
        public string Caption { get; set; }

        public MessageMediaGroup()
        {
            this.Files = new List<FileData>();

        }

        public MessageMediaGroup(List<FileData> files)
        {
            this.Files = files;
        }


    }
}
