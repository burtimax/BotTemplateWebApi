using System;

namespace MultipleBotFramework.Exceptions
{
    public class UserPropertiesHasNotValueByKeyException : Exception
    {
        public UserPropertiesHasNotValueByKeyException(string key) : base($"User properties has not value by key [{key}]")
        {
            
        }
    }
}