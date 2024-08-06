using System;

namespace MultipleBotFrameworkUpgrade.Exceptions
{
    public class CannotCreateUpdateHandlerInstanceException : Exception
    {
        public CannotCreateUpdateHandlerInstanceException(string? handlerType, string? assemblyName) 
            : base($"Cannot create handler instance. Handler type [{handlerType}] in assembly [{assemblyName}].")
        {
            
        }
    }
}