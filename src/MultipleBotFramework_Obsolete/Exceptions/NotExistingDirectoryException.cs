using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class NotExistingDirectoryException : Exception
    {
        public NotExistingDirectoryException(string directory) : 
            base($"Directory [{directory}] not exists")
        {
            
        }
    }
}