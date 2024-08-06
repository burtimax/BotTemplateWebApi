using System;

namespace MultipleBotFrameworkUpgrade.Exceptions
{
    public class InvalidFilePathException : Exception
    {
        public InvalidFilePathException(string filePath) : 
            base($"File path [{filePath}] is invalid")
        {
        }
    }
}