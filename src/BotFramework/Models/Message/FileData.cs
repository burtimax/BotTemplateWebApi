using System.IO;
using System.Threading.Tasks;
using BotFramework.Exceptions;
using File = Telegram.Bot.Types.File;

namespace BotFramework.Models.Message
{
    /// <summary>
    /// Базовый класс файла (информацию о файле + данные файла)
    /// </summary>
    public class FileData
    {
        /// <summary>
        /// Информация о файле
        /// </summary>
        public File Info { get; set; }
        
        /// <summary>
        /// Поток для работы с файлом
        /// </summary>
        public MemoryStream? Stream
        {
            get;
            set;
        }

        //ToDo Не нравится мне это свойство, убрать и оставить только stream
        /// <summary>
        /// Свойство-обертка над потоком для удобства работы с файлом
        /// </summary>
        public byte[] Data
        {
            // set
            // {
            //     this.Stream = new MemoryStream(value);
            // }
            get
            {
                return this.Stream.ToArray();
            }
        }

        /// <summary>
        /// Сохранить файл по полному наименованию
        /// </summary>
        /// <param name="filePath">Полный путь к файлу, который нужно сохранить</param>
        public async Task SaveFile(FilePath filePath)
        {
            if(IsNullData())
            {
                throw new FileNullDataException();
            }

            if (filePath.IsDirectoryExists() == false)
            {
                throw new NotExistingDirectoryException(filePath.DirectoryName);
            }
            
            DeleteExistedFile(filePath);
            
            await System.IO.File.WriteAllBytesAsync(filePath, this.Data);
        }

        private bool IsNullData() => Stream == null || Stream.Length == 0;

        private void DeleteExistedFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        
    }
}
