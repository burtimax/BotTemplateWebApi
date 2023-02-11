using System;

namespace BotFramework.Exceptions
{
    public class UnsupportedMessageTypeException : Exception
    {
        public UnsupportedMessageTypeException(string messageType)
        : base($"Unsupported message type [{messageType}]")
        {
        }
    }
}