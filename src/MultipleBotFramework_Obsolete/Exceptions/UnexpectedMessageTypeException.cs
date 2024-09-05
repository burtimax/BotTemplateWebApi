using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class UnexpectedMessageTypeException : Exception
    {
        public UnexpectedMessageTypeException() : 
            base("Unexpected message type")
        {
            
        }
    }
}