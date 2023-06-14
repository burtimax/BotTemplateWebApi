using System;

namespace BotFramework.Exceptions
{
    public class UnexpectedMessageTypeException : Exception
    {
        public UnexpectedMessageTypeException() : 
            base("Unexpected message type")
        {
            
        }
    }
}