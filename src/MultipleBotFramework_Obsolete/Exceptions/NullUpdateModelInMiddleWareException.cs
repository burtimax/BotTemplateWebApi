using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class NullUpdateModelInMiddleWareException : Exception
    {
        public NullUpdateModelInMiddleWareException()
        : base($"Update model is null in middleware!")
        {
        }
    }
}