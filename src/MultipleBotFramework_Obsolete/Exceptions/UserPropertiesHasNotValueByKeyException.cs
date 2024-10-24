using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class UserPropertiesHasNotValueByKeyException : Exception
    {
        public UserPropertiesHasNotValueByKeyException(string key) : base($"User properties has not value by key [{key}]")
        {
            
        }
    }
}