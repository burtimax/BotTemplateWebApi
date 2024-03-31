using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Services;

public class SavedMessageService : ISavedMessageService
{
    private readonly BotDbContext _db;
    
    public SavedMessageService(BotDbContext db)
    {
        _db = db;
    }

    public async Task<BotSavedMessage> SaveMessageFromUpdate(BotChat? chat, BotUser? user, Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        BotSavedMessage savedMessage = new()
        {
            TelegramChatId = chat?.TelegramId,
            TelegramUserId = user?.TelegramId,
            MediaGroupId = string.IsNullOrEmpty(message.MediaGroupId) ? Guid.NewGuid().ToString() : message.MediaGroupId,
            TelegramMessageId = message.MessageId,
            TelegramMessage = message,
            Comment = "",
        };

        _db.Add(savedMessage);
        await _db.SaveChangesAsync();

        return savedMessage;
    }

    public async Task<bool> HasSavedMessageWithMediaType(long? telegramChatId, long? telegramUserId, string? mediaGroupId)
    {
        return 0 < ( await _db.SavedMessages.CountAsync(sm => sm.TelegramChatId == telegramChatId &&
                                                  sm.TelegramUserId == telegramUserId &&
                                                  sm.MediaGroupId == mediaGroupId));
    }
    
}