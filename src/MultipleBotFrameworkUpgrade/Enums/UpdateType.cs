namespace MultipleBotFrameworkUpgrade.Enums;


public enum UpdateType
{
    Unknown = 0,
    Message,
    Command,
    InlineQuery,
    ChosenInlineResult,
    CallbackQuery,
    EditedMessage,
    ChannelPost,
    EditedChannelPost,
    ShippingQuery,
    PreCheckoutQuery,
    Poll,
    PollAnswer,
    MyChatMember,
    ChatMember,
    ChatJoinRequest,
}
