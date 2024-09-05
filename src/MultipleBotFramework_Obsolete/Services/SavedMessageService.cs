using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework_Obsolete.Db;
using MultipleBotFramework_Obsolete.Db.Entity;
using Telegram.Bot.Types;

namespace MultipleBotFramework_Obsolete.Services;

public class SavedMessageService : ISavedMessageService
{
    private readonly BotDbContext _db;
    
    public SavedMessageService(BotDbContext db)
    {
        _db = db;
    }

    public async Task<BotSavedMessageEntity> SaveMessageFromUpdate(long botId, BotChatEntity? chat, BotUserEntity? user, Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        BotSavedMessageEntity savedMessageEntity = new()
        {
            TelegramChatId = chat?.TelegramId,
            TelegramUserId = user?.TelegramId,
            MediaGroupId = string.IsNullOrEmpty(message.MediaGroupId) ? Guid.NewGuid().ToString() : message.MediaGroupId,
            TelegramMessageId = message.MessageId,
            TelegramMessage = message,
            Comment = "",
        };

        _db.Add(savedMessageEntity);
        await _db.SaveChangesAsync();

        return savedMessageEntity;
    }

    public async Task<bool> HasSavedMessageWithMediaType(long botId, long? telegramChatId, long? telegramUserId, string? mediaGroupId)
    {
        return 0 < ( await _db.SavedMessages.CountAsync(sm => sm.BotId == botId &&
                                                              sm.TelegramChatId == telegramChatId &&
                                                              sm.TelegramUserId == telegramUserId &&
                                                              sm.MediaGroupId == mediaGroupId));
    }
    
}