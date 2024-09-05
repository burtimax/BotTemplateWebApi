using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class NotExistingFileException : Exception
    {
        public NotExistingFileException(string filePath) : 
            base($"File [{filePath}] not exists")
        {
            
        }
    }
}