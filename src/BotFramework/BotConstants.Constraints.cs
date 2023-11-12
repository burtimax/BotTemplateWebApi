using System.Collections.Generic;
using BotFramework.Db.Entity;

namespace BotFramework
{
    public partial class BotConstants
    {
        /// <summary>
        /// Телеграм ограничения в константах.
        /// </summary>
        public class Constraints
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
    }
}