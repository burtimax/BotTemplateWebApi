using System.Linq;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using Newtonsoft.Json;
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

    public static object GetPayload(this Message m)
    {
        return m.Type() switch
        {
            MessageType.Audio => m.Audio,
            MessageType.Contact => m.Contact,
            MessageType.Text => m.Text,
            MessageType.Dice => m.Dice,
            MessageType.Document => m.Document,
            MessageType.Game => m.Game,
            MessageType.Invoice => m.Invoice,
            MessageType.Location => m.Location,
            MessageType.Photo => m.Photo,
            MessageType.Poll => m.Poll,
            MessageType.Sticker => m.Sticker,
            MessageType.Venue => m.Venue,
            MessageType.Video => m.Video,
            MessageType.Voice => m.Voice,
            MessageType.ChannelCreated =>  m.ChannelChatCreated,
            MessageType.GroupCreated => m.GroupChatCreated,
            MessageType.MessagePinned => m.PinnedMessage,
            MessageType.SuccessfulPayment => m.SuccessfulPayment,
            MessageType.SupergroupCreated => m.SupergroupChatCreated,
            MessageType.VideoNote => m.VideoNote,
            MessageType.WebsiteConnected => m.ConnectedWebsite,
            MessageType.ChatMemberLeft => m.LeftChatMember,
            MessageType.ChatMembersAdded => m.NewChatMembers,
            MessageType.ChatPhotoChanged => m.NewChatPhoto,
            MessageType.ChatPhotoDeleted => m.DeleteChatPhoto ,
            MessageType.ChatTitleChanged => m.NewChatTitle, 
            MessageType.MigratedFromGroup => new{ From = m.MigrateFromChatId, To = m.MigrateToChatId },
            MessageType.MigratedToSupergroup => new { From = m.MigrateFromChatId, To = m.MigrateToChatId },
            MessageType.ProximityAlertTriggered => m.ProximityAlertTriggered,
            MessageType.VideoChatEnded => m.VideoChatEnded,
            MessageType.VideoChatScheduled => m.VideoChatScheduled,
            MessageType.VideoChatStarted => m.VideoChatStarted,
            MessageType.WebAppData => m.WebAppData,
            MessageType.VideoChatParticipantsInvited => m.VideoChatParticipantsInvited,
            MessageType.MessageAutoDeleteTimerChanged => m.MessageAutoDeleteTimerChanged,
            MessageType.WriteAccessAllowed => m.WriteAccessAllowed?.WebAppName ?? "NULL",
            MessageType.Unknown => new{ Content = nameof(MessageType.Unknown) },
            _ => new{ Value = "NOT IMPLEMENTED MESSAGE TYPE" },
        };
    }
    
    public static bool TrySetContentToChatHistory(this Message m, ref BotChatHistoryEntity item)
    {
        item.Type = ChatHistoryType.Message;
        item.MessageId = m.MessageId;
        item.MediaGroupId = m.MediaGroupId;
        item.MessageType = UpdateType.Message.ToString() + "." + m.Type().ToString();
        item.JsonObject = GetPayload(m)?.ToJson();

        string content = $"#[{m.Type().ToString()}]\n";
        string? fileId = "";
        
        switch(m.Type())
        {
            case MessageType.Audio: fileId = m.Audio!.FileId; content += m.Audio.Title ?? m.Audio.FileName; break;
            case MessageType.Contact: content += m.Contact.FirstName + " " + m.Contact.LastName + " " + m.Contact.PhoneNumber; break;
            case MessageType.Text: content += m.Text; break;
            case MessageType.Dice: content += m.Dice.Value; break;
            case MessageType.Document: fileId = m.Document.FileId; content += "Имя файла: " + m.Document.FileName + ". Размер: " + m.Document.FileSize; break;
            case MessageType.Game: content += $"{m.Game.Title} ({m.Game.Description})"; break;
            case MessageType.Invoice: content += $"Сумма (в копейках) {m.Invoice.TotalAmount + "/" + m.Invoice.Currency} на товар {m.Invoice.Title} ({m.Invoice.Description})"; break;
            case MessageType.Location: content += $"Широта:{m.Location.Latitude}, долгота:{m.Location.Longitude}"; break;
            case MessageType.Photo: fileId = m.Photo.First().FileId; content += $"{m.Caption}"; break;
            case MessageType.Poll: content += m.Poll.Question; break;
            case MessageType.Sticker: fileId = m.Sticker.FileId; content += m.Sticker.Emoji; break;
            case MessageType.Venue: content += $"{m.Venue.Title ?? "TITLE"}, {m.Venue.Address ?? "ADDRESS"}"; break;
            case MessageType.Video: fileId = m.Video.FileId; content += $"({m.Video.FileName}) {m.Caption}"; break;
            case MessageType.Voice: fileId = m.Voice.FileId; content += $"({m.Voice.Duration} сек.) {m.Caption}"; break;
            // case MessageType.ChannelCreated:  m.ChannelChatCreated, break;
            // case MessageType.GroupCreated: m.GroupChatCreated, break;
            // case MessageType.MessagePinned: m.PinnedMessage, break;
            // case MessageType.SuccessfulPayment: m.SuccessfulPayment, break;
            // case MessageType.SupergroupCreated: m.SupergroupChatCreated, break;
            // case MessageType.VideoNote: m.VideoNote, break;
            // case MessageType.WebsiteConnected: m.ConnectedWebsite, break;
            // case MessageType.ChatMemberLeft: m.LeftChatMember, break;
            // case MessageType.ChatMembersAdded: m.NewChatMembers, break;
            // case MessageType.ChatPhotoChanged: m.NewChatPhoto,
            // case MessageType.ChatPhotoDeleted: m.DeleteChatPhoto , break;
            // case MessageType.ChatTitleChanged: m.NewChatTitle,  break;
            // case MessageType.MigratedFromGroup: new{ From = m.MigrateFromChatId, To = m.MigrateToChatId }, break;
            // case MessageType.MigratedToSupergroup: new { From = m.MigrateFromChatId, To = m.MigrateToChatId }, break;
            // case MessageType.ProximityAlertTriggered: m.ProximityAlertTriggered, break;
            // case MessageType.VideoChatEnded: m.VideoChatEnded, break;
            // case MessageType.VideoChatScheduled: m.VideoChatScheduled, break;
            // case MessageType.VideoChatStarted: m.VideoChatStarted, break;
            // case MessageType.WebAppData: m.WebAppData, break;
            // case MessageType.VideoChatParticipantsInvited: m.VideoChatParticipantsInvited, break;
            // case MessageType.MessageAutoDeleteTimerChanged: m.MessageAutoDeleteTimerChanged, break;
            // case MessageType.WriteAccessAllowed: m.WriteAccessAllowed?.WebAppName ?? "NULL", break;
            // case MessageType.Unknown: new{ Content = nameof(MessageType.Unknown) }, break;
            default: return false; 
        };

        item.FileId = fileId;
        item.Content = content;
        
        return true;
    }
    
}