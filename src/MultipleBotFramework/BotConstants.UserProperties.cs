namespace MultipleBotFramework
{
    public partial class BotConstants
    {
        /// <summary>
        /// Класс, определяющий свойства и атрибуты, связанные с администраторами системы.
        /// Используется для хранения и управления специфическими данными администраторов.
        /// </summary>
        public class AdminProperties
        {
            /// <summary>
            /// Ключ для хранения последнего использованного пароля администратора.
            /// Используется для отслеживания и управления паролями администраторов.
            /// </summary>
            public const string LastPasswordProperty = "last_password";
        }
    }
}