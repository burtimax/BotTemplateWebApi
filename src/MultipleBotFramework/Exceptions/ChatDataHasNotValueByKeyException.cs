using System;

namespace MultipleBotFramework.Exceptions
{
    public class ChatDataHasNotValueByKeyException : Exception
    {
        public ChatDataHasNotValueByKeyException(string key) : base($"Chat data has not value by key [{key}]")
        {
            
        }
    }
}