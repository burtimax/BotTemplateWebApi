namespace BotFramework
{
    public class BotConstants
    {
        /// <summary>
        /// Основные пользовательские роли.
        /// </summary>
        public class UserRoles
        {
            public const string User = "user";
            public const string Admin = "admin";
            public const string Moderator = "moderator";
        }

        /// <summary>
        /// Пользовательские свойства администратора.
        /// </summary>
        public class AdminClaims
        {
            public const string LastPasswordClaim = "last_password";
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