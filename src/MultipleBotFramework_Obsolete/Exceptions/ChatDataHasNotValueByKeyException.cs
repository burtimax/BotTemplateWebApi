using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class ChatDataHasNotValueByKeyException : Exception
    {
        public ChatDataHasNotValueByKeyException(string key) : base($"Chat data has not value by key [{key}]")
        {
            
        }
    }
}