using System;

namespace MultipleBotFrameworkUpgrade.Exceptions
{
    public class UnexpectedMessageTypeException : Exception
    {
        public UnexpectedMessageTypeException() : 
            base("Unexpected message type")
        {
            
        }
    }
}