using System;

namespace MultipleBotFramework.Exceptions
{
    public class InvalidFilePathException : Exception
    {
        public InvalidFilePathException(string filePath) : 
            base($"File path [{filePath}] is invalid")
        {
        }
    }
}