using System;

namespace BotFramework.Exceptions
{
    public class UnknownUpdateTypeException : Exception
    {
        public UnknownUpdateTypeException() : 
            base("Unknown update type.")
        {
            
        }
    }
}