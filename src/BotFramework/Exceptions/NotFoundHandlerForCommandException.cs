using System;

namespace BotFramework.Exceptions
{
    public class NotFoundHandlerForCommandException : Exception
    {
        public NotFoundHandlerForCommandException(string? botCommand, string? assemblyName) 
            : base($"Not found bot handler type for command [{botCommand}] in assembly [{assemblyName}]")
        {
            
        }
    }
}