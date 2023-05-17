using System;

namespace BotFramework.Exceptions
{
    public class NotFoundHandlerTypeException : Exception
    {
        public NotFoundHandlerTypeException(string? handlerType, string? assemblyName) 
            : base($"Not found bot handler type [{handlerType}] in assembly [{assemblyName}]")
        {
            
        }
    }
}