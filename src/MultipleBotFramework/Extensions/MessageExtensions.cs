using MultipleBotFramework.Enums;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Extensions;

public static class MessageExtensions
{
    public static MessageType Type(this Message message)
    {
        return message switch
        {
            { Text: { } }                          => MessageType.Text,
            { Photo: { } }                         => MessageType.Photo,
            { Audio: { } }                         => MessageType.Audio,
            { Video: { } }                         => MessageType.Video,
            { Voice: { } }                         => MessageType.Voice,
            { Animation: { } }                     => MessageType.Animation,
            { Document: { } }                      => MessageType.Document,
            { Sticker: { } }                       => MessageType.Sticker,
            // Venue also contains Location
            { Location: { } } and { Venue: null }  => MessageType.Location,
            { Venue: { } }                         => MessageType.Venue,
            { Contact: { } }                       => MessageType.Contact,
            { Game: { } }                          => MessageType.Game,
            { VideoNote: { } }                     => MessageType.VideoNote,
            { Invoice: { } }                       => MessageType.Invoice,
            { SuccessfulPayment: { } }             => MessageType.SuccessfulPayment,
            { ConnectedWebsite: { } }              => MessageType.WebsiteConnected,
            { NewChatMembers: { /*Length: > 0*/ } }    => MessageType.ChatMembersAdded,
            { LeftChatMember: { } }                => MessageType.ChatMemberLeft,
            { NewChatTitle: { } }                  => MessageType.ChatTitleChanged,
            { NewChatPhoto: { } }                  => MessageType.ChatPhotoChanged,
            { PinnedMessage: { } }                 => MessageType.MessagePinned,
            { DeleteChatPhoto: { } }               => MessageType.ChatPhotoDeleted,
            { GroupChatCreated: { } }              => MessageType.GroupCreated,
            { SupergroupChatCreated: { } }         => MessageType.SupergroupCreated,
            { ChannelChatCreated: { } }            => MessageType.ChannelCreated,
            { MigrateToChatId: { } }               => MessageType.MigratedToSupergroup,
            { MigrateFromChatId: { } }             => MessageType.MigratedFromGroup,
            { Poll: { } }                          => MessageType.Poll,
            { Dice: { } }                          => MessageType.Dice,
            { MessageAutoDeleteTimerChanged: { } } => MessageType.MessageAutoDeleteTimerChanged,
            { ProximityAlertTriggered: { } }       => MessageType.ProximityAlertTriggered,
            { VideoChatScheduled: { } }            => MessageType.VideoChatScheduled,
            { VideoChatStarted: { } }              => MessageType.VideoChatStarted,
            { VideoChatEnded: { } }                => MessageType.VideoChatEnded,
            { VideoChatParticipantsInvited: { } }  => MessageType.VideoChatParticipantsInvited,
            { WebAppData: { } }                    => MessageType.WebAppData,
            { ForumTopicCreated: { } }             => MessageType.ForumTopicCreated,
            { ForumTopicEdited: { } }              => MessageType.ForumTopicEdited,
            { ForumTopicClosed: { } }              => MessageType.ForumTopicClosed,
            { ForumTopicReopened: { } }            => MessageType.ForumTopicReopened,
            { GeneralForumTopicHidden: { } }       => MessageType.GeneralForumTopicHidden,
            { GeneralForumTopicUnhidden: { } }     => MessageType.GeneralForumTopicUnhidden,
            { WriteAccessAllowed: { } }            => MessageType.WriteAccessAllowed,
            { UsersShared: { } }                    => MessageType.UserShared,
            { ChatShared: { } }                    => MessageType.ChatShared,
            _                                      => MessageType.Unknown
        };
    }

    
}