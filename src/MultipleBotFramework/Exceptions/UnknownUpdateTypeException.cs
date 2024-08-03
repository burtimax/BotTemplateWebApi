using System;

namespace MultipleBotFramework.Exceptions
{
    public class UnknownUpdateTypeException : Exception
    {
        public UnknownUpdateTypeException() : 
            base("Unknown update type.")
        {
            
        }
    }
}