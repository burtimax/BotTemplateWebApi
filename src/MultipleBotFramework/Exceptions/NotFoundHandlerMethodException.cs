using System;

namespace MultipleBotFramework.Exceptions
{
    public class NotFoundHandlerMethodException : Exception
    {
        public NotFoundHandlerMethodException(string? handlerMethodName, string? handlerType, string? assemblyName) 
            : base($"Not found bot handler method [{handlerMethodName}] in type [{handlerType}] in assembly [{assemblyName}].")
        {
            
        }
    }
}