namespace MultipleBotFramework
{
    public partial class BotConstants
    {
        public const string StartState = "StartState";
      
        /// <summary>
        /// https://t.me/{USERNAME}
        /// </summary>
        public const string TelegramLinkFormat = "https://t.me/{0}";
        
        public class ModerationChatTags
        {
            public const string Exception = "exception";
            public const string Event = "event";
        }
    }
}