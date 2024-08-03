using System;

namespace MultipleBotFramework.Exceptions
{
    public class NotFoundBotClaim : Exception
    {
        public NotFoundBotClaim(string claimName) 
            : base($"Not found bot claim [{claimName}] in database.")
        {
            
        }
    }
}