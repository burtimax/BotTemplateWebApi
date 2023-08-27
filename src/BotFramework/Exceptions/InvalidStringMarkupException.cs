using System;

namespace BotFramework.Exceptions
{
    public class InvalidStringMarkupException : Exception
    {
        public InvalidStringMarkupException(string str) : 
            base($"String Markup \"{str}\" is invalid")
        {
        }
    }
}