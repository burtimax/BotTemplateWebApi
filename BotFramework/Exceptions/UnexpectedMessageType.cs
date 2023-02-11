using System;

namespace BotFramework.Exceptions
{
    public class UnexpectedMessageType : Exception
    {
        public UnexpectedMessageType() : 
            base("Unexpected message type")
        {
            
        }
    }
}