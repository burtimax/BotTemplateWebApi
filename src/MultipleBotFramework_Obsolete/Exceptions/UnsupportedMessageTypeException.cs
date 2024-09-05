using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class UnsupportedMessageTypeException : Exception
    {
        public UnsupportedMessageTypeException(string messageType)
        : base($"Unsupported message type [{messageType}]")
        {
        }
    }
}