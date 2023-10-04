using System.Collections.Generic;
using BotFramework.Db.Entity;

namespace BotFramework
{
    public partial class BotConstants
    {
        /// <summary>
        /// Пользовательские свойства администратора.
        /// </summary>
        public class AdminProperties
        {
            public const string LastPasswordProperty = "last_password";
        }
    }
}