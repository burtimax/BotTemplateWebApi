using System;
using System.IO;
using System.Threading.Tasks;
using BotFramework.Exceptions;

namespace BotFramework.Models.Message
{
    /// <summary>
    /// Базовый класс для файла Telegram 
    /// </summary>
    /// <remarks>Используется при отправке и приеме файлов различных типов в Telegram</remarks>
    public class MessageData
    {
        /// <summary>
        /// Файл
        /// </summary>
        public FileData File { get; set; }
        
        /// <summary>
        /// Заголовок файла
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Асинхронный фабричный метод
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <typeparam name="T">Тип,наследник класса MessageData</typeparam>
        /// <returns>Новый экземпляр класса T</returns>
        /// <exception cref="NotExistingFileException"></exception>
        public static async Task<T> GetNewInstanceAsync<T>(FilePath filePath) where T : MessageData, new()
        {
            if (filePath.IsFileExists() == false)
            {
                throw new NotExistingFileException(filePath);
            }

            T instance = new T();
            instance.File = new FileData();
            instance.File.Stream = new MemoryStream(await System.IO.File.ReadAllBytesAsync(filePath));
            instance.File.Info = new Telegram.Bot.Types.File();
            instance.File.Info.FilePath = filePath;
            instance.File.Info.FileSize = instance.File.Data.Length;

            return instance;
        }
        
        /// <summary>
        /// Сохранить данные в файл
        /// </summary>
        /// <param name="filePath"></param>
        public async Task SaveDataToFileAsync(FilePath filePath )
        {
            if (filePath.IsDirectoryExists() == false)
            {
                throw new NotExistingDirectoryException(filePath.DirectoryName);
            }

            using (FileStream fs = new(filePath, FileMode.OpenOrCreate))
            {
                 await fs.WriteAsync(this.File.Data,0,this.File.Data.Length);   
            }
        }

        /// <summary>
        /// Считываем данные из файла
        /// </summary>
        /// <param name="filePath">Путь к файлу</param>
        public async Task ReadDataFromFileAsync(FilePath filePath)
        {
            if (filePath.IsFileExists() == false)
            {
                throw new NotExistingFileException(filePath);
            }

            using (FileStream fs = new (filePath, FileMode.Open))
            {
                await fs.ReadAsync(this.File.Data, 0, Convert.ToInt32(fs.Length));
                await fs.CopyToAsync(this.File.Stream);
            }
        }
    }
}
