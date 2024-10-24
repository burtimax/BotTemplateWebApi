using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class UnknownUpdateTypeException : Exception
    {
        public UnknownUpdateTypeException() : 
            base("Unknown update type.")
        {
            
        }
    }
}