using System;

namespace MultipleBotFramework_Obsolete.Exceptions
{
    public class NotFoundHandlerForStateException : Exception
    {
        public NotFoundHandlerForStateException(string? stateName, string? assemblyName) 
            : base($"Not found bot handler type for state [{stateName}] in assembly [{assemblyName}]")
        {
            
        }
    }
}