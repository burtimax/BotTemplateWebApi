using System;

namespace MultipleBotFrameworkUpgrade.Exceptions
{
    public class UnknownUpdateTypeException : Exception
    {
        public UnknownUpdateTypeException() : 
            base("Unknown update type.")
        {
            
        }
    }
}