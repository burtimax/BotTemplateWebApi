using System.IO;
using BotFramework.Exceptions;

namespace BotFramework.Models
{
    /// <summary>
    /// Класс для работы с именем файла
    /// </summary>
    public class FilePath
    {

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="fullFilePath">Полный путь файла</param>
        /// <exception cref="InvalidFilePathException">Неправильный путь файла</exception>
        public FilePath(string fullFilePath)
        {
            FullFilePath = fullFilePath;
            DirectoryName = Path.GetDirectoryName(FullFilePath);
            FileName = Path.GetFileName(FullFilePath);
            Extension = Path.GetExtension(FullFilePath);
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FullFilePath);
            
            if (IsValid() == false)
            {
                throw new InvalidFilePathException(FullFilePath);
            }
        }

        /// <summary>
        /// Например: C:\Users\Important\Documents\File.txt
        /// </summary>
        private string FullFilePath { get; init; }
        
        /// <summary>
        /// Например: C:\Users\Important\Documents
        /// </summary>
        public string DirectoryName { get; init; }
        
        /// <summary>
        /// Например: File.txt
        /// </summary>
        public string FileName { get; init; }
        
        /// <summary>
        /// Например: .txt
        /// </summary>
        public string Extension { get; init; }
        
        /// <summary>
        /// Например: File
        /// </summary>
        public string FileNameWithoutExtension { get; init; }

        /// <summary>
        /// Валидные ли путь файла
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            return
                string.IsNullOrEmpty(DirectoryName) == false &&
                string.IsNullOrEmpty(FileNameWithoutExtension) == false &&
                string.IsNullOrEmpty(Extension) == false &&
                FullFilePath.IndexOfAny(Path.GetInvalidPathChars()) < 0;

        }

        /// <summary>
        /// Проверка существования директории
        /// </summary>
        public bool IsDirectoryExists()
        {
            return Directory.Exists(DirectoryName);
        }
        
        /// <summary>
        /// Проверка существования файла
        /// </summary>
        public bool IsFileExists()
        {
            return System.IO.File.Exists(FullFilePath);
        }

        public static implicit operator string(FilePath filePath)
        {
            return filePath.FullFilePath;
        }
    }
}