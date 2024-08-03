using System;

namespace MultipleBotFramework.Exceptions
{
    public class UnexpectedMessageTypeException : Exception
    {
        public UnexpectedMessageTypeException() : 
            base("Unexpected message type")
        {
            
        }
    }
}