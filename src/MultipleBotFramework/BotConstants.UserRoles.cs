namespace MultipleBotFramework
{
    public partial class BotConstants
    {
        /// <summary>
        /// Класс, определяющий основные роли пользователей в системе.
        /// Используется для управления доступом и правами пользователей.
        /// </summary>
        public class UserRoles
        {
            /// <summary>
            /// Роль обычного пользователя.
            /// Имеет базовые права доступа к функционалу бота.
            /// </summary>
            public const string User = "user";

            /// <summary>
            /// Роль администратора.
            /// Имеет полный доступ ко всем функциям бота и может управлять другими пользователями.
            /// </summary>
            public const string Admin = "admin";

            /// <summary>
            /// Роль модератора.
            /// Имеет расширенные права для управления контентом и пользователями,
            /// но с некоторыми ограничениями по сравнению с администратором.
            /// </summary>
            public const string Moderator = "moderator";
        }
    }
}