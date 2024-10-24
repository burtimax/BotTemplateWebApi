using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class InvalidStringMarkupException : Exception
    {
        public InvalidStringMarkupException(string str) : 
            base($"String Markup \"{str}\" is invalid")
        {
        }
    }
}