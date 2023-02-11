using System;

namespace BotFramework.Exceptions
{
    public class NotExistingDirectoryException : Exception
    {
        public NotExistingDirectoryException(string directory) : 
            base($"Directory [{directory}] not exists")
        {
            
        }
    }
}