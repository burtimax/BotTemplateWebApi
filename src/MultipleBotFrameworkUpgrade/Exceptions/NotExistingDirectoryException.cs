﻿using System;

namespace MultipleBotFrameworkUpgrade.Exceptions
{
    public class NotExistingDirectoryException : Exception
    {
        public NotExistingDirectoryException(string directory) : 
            base($"Directory [{directory}] not exists")
        {
            
        }
    }
}