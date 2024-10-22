using System;
using System.Threading.Tasks;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services.Interfaces;
using Newtonsoft.Json;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Services;

/// <inheritdoc />
public class SaveUpdateService
{
    private BotDbContext _db;

    public SaveUpdateService(BotDbContext db)
    {
        _db = db;
    }
    
    /// <inheritdoc />
    public async Task<BotUpdateEntity> SaveUpdateInBotHistory(long botId, BotUserEntity? user, BotChatEntity? chat, Update update)
    {
        SaveUpdateDto saveUpdateDto = new()
        {
            BotChatId = chat?.Id,
            BotUserId = user?.Id,
            Type = update.Type().ToString(),
            TelegramId = update?.Message?.MessageId ?? update.UpdateId,
            Content = "NULL"
        };

        if (update.Type() == UpdateType.Message ||
            update.Type() == UpdateType.Command)
        {
            saveUpdateDto.Type += "." + update.Message.Type().ToString();
        }

        saveUpdateDto.Content = update.GetPayload()?.ToJson() ?? "NULL";

        if (update.Type() == UpdateType.Message ||
            update.Type() == UpdateType.Command)
        {
            saveUpdateDto.Content = update.ToJson(); //GetContentByMessageType(update.Message);
        }
        
        saveUpdateDto.Content ??= "NULL";

        BotUpdateEntity newUpdateEntity = new BotUpdateEntity()
        {
            BotId = botId,
            TelegramMessageId = saveUpdateDto.TelegramId,
            ChatId = saveUpdateDto.BotChatId,
            UserId = saveUpdateDto.BotUserId,
            Type = saveUpdateDto.Type,
            Content = saveUpdateDto.Content
        };

        newUpdateEntity.CreatedAt = DateTimeOffset.Now;
        _db.Updates.Add(newUpdateEntity);
        await _db.SaveChangesAsync();
        return newUpdateEntity;
    }

    /// <summary>
    /// Получить сериализованный в json объект сообщения.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private string GetContentByMessageType(Message message)
    {
        return JsonConvert.SerializeObject(GetObjectByMessageType(message), Formatting.Indented);
    }

    /// <summary>
    /// Получить объект с данными в зависимости от типа сообщения.
    /// </summary>
    /// <param name="message">Сообщение.</param>
    /// <returns></returns>
    private object? GetObjectByMessageType(Message message)
    {
        return message.Type() switch
        {
            MessageType.Audio => message.Audio,
            MessageType.Contact => message.Contact,
            MessageType.Text => message.Text,
            MessageType.Dice => message.Dice,
            MessageType.Document => message.Document,
            MessageType.Game => message.Game,
            MessageType.Invoice => message.Invoice,
            MessageType.Location => message.Location,
            MessageType.Photo => message.Photo,
            MessageType.Poll => message.Poll,
            MessageType.Sticker => message.Sticker,
            MessageType.Venue => message.Venue,
            MessageType.Video => message.Video,
            MessageType.Voice => message.Voice,
            MessageType.ChannelCreated =>  new { Value = message.ChannelChatCreated },
            MessageType.GroupCreated => new{ Value = message.GroupChatCreated },
            MessageType.MessagePinned => message.PinnedMessage,
            MessageType.SuccessfulPayment => message.SuccessfulPayment,
            MessageType.SupergroupCreated => new{ Value = message.SupergroupChatCreated },
            MessageType.VideoNote => message.VideoNote,
            MessageType.WebsiteConnected => message.ConnectedWebsite,
            MessageType.ChatMemberLeft => message.LeftChatMember,
            MessageType.ChatMembersAdded => message.NewChatMembers,
            MessageType.ChatPhotoChanged => message.NewChatPhoto,
            MessageType.ChatPhotoDeleted => new { Value = message.DeleteChatPhoto },
            MessageType.ChatTitleChanged => new { Value = message.NewChatTitle }, 
            MessageType.MigratedFromGroup => new{ From = message.MigrateFromChatId, To = message.MigrateToChatId },
            MessageType.MigratedToSupergroup => new { From = message.MigrateFromChatId, To = message.MigrateToChatId },
            MessageType.ProximityAlertTriggered => message.ProximityAlertTriggered,
            MessageType.VideoChatEnded => message.VideoChatEnded,
            MessageType.VideoChatScheduled => message.VideoChatScheduled,
            MessageType.VideoChatStarted => message.VideoChatStarted,
            MessageType.WebAppData => message.WebAppData,
            MessageType.VideoChatParticipantsInvited => message.VideoChatParticipantsInvited,
            MessageType.MessageAutoDeleteTimerChanged => message.MessageAutoDeleteTimerChanged,
            MessageType.WriteAccessAllowed => new { Value = message.WriteAccessAllowed?.WebAppName ?? "NULL" },
            MessageType.Unknown => new{ Content = nameof(MessageType.Unknown) },
            _ => new{ Value = "NOT IMPLEMENTED MESSAGE TYPE" },
        };
    }
        
}