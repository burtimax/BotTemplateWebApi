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
public class SaveUpdateService : ISaveUpdateService
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
            BotChatId = chat?.Id ?? 0,
            Type = update.Type().ToString(),
            TelegramId = update.UpdateId,
            Content = "NULL"
        };

        saveUpdateDto.Content = update.Type() switch
        {
            UpdateType.Message => GetContentByMessageType(update.Message), 
            UpdateType.Command => GetContentByMessageType(update.Message), 
            UpdateType.CallbackQuery => update.CallbackQuery?.Data ?? "",
            UpdateType.EditedMessage => update.EditedMessage?.Text ?? "",
            UpdateType.Poll => update.Poll?.Question ?? "",
            UpdateType.ChannelPost => $"{update.ChannelPost?.Caption} \n {update.ChannelPost?.Text}",
            UpdateType.ChatMember => $"{update.ChatMember?.From?.Id} : {update.ChatMember?.From?.Username}",
            UpdateType.InlineQuery => update.InlineQuery?.Query ?? "",
            UpdateType.PollAnswer => update.PollAnswer?.PollId ?? "",
            UpdateType.ShippingQuery => update.ShippingQuery?.InvoicePayload ?? "",
            UpdateType.ChatJoinRequest => update.ChatJoinRequest?.Bio ?? "",
            UpdateType.ChosenInlineResult => update.ChosenInlineResult?.Query ?? "",
            UpdateType.EditedChannelPost => $"{update.EditedChannelPost?.Caption} \n {update.EditedChannelPost?.Text}",
            UpdateType.MyChatMember => $"{update.MyChatMember?.From?.Id} : {update.MyChatMember?.From?.Username}",
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery?.InvoicePayload ?? "",
            UpdateType.Unknown => "UNKNOWN UPDATE",
        };
        
        saveUpdateDto.Content ??= "";

        BotUpdateEntity newUpdateEntity = new BotUpdateEntity()
        {
            BotId = botId,
            TelegramMessageId = saveUpdateDto.TelegramId,
            ChatId = saveUpdateDto.BotChatId,
            Type = saveUpdateDto.Type,
            Content = saveUpdateDto.Content
        };

        newUpdateEntity.CreatedAt = DateTimeOffset.Now;
        _db.Updates.Add(newUpdateEntity);
        await _db.SaveChangesAsync();
        return newUpdateEntity;
    }

    public async Task<BotUpdateEntity> SaveBotRequestResult<TResult>(string method, TResult result)
    {
        // TODO 
        throw new NotImplementedException();
    }

    /// <summary>
    /// Получить сериализованный в json объект сообщения.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private string GetContentByMessageType(Message message)
    {
        return JsonConvert.SerializeObject(new{ MessageType = message.Type().ToString(), Content = GetObjectByMessageType(message)}, Formatting.Indented);
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
            MessageType.Unknown => new{ Content = nameof(MessageType.Unknown) },
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
            _ => new{ Value = "UNKNOWN MESSAGE TYPE" },
        };
    }
        
}