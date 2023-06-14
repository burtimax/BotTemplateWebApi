namespace BotFramework
{
    public class BotConstants
    {
        public class UserRoles
        {
            public const string User = "user";
        }

        /// <summary>
        /// Телеграм ограничения в константах.
        /// </summary>
        public class Bounds
        {
            /// <summary>
            /// Ограничение по длине сообщения.
            /// </summary>
            public const int MaxMessageLength = 4096;
            
            /// <summary>
            /// Максимальная длина описания к документу.
            /// </summary>
            public const int MaxDocumentCaption = 1024;
        }

        public const string StartState = "StartState";
    }
}