using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Services.Interfaces;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Services;

/// <summary>
/// Сервис для сохранения и проверки сохранённых сообщений.
/// </summary>
public class SavedMessageService : ISavedMessageService
{
    private readonly BotDbContext _db;
    
    /// <summary>
    /// Конструктор сервиса сохранённых сообщений.
    /// </summary>
    /// <param name="db">Контекст базы данных бота</param>
    public SavedMessageService(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Сохраняет сообщение из Telegram-обновления.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="chat">Сущность чата</param>
    /// <param name="user">Сущность пользователя</param>
    /// <param name="message">Сообщение Telegram</param>
    /// <returns>Сущность сохранённого сообщения</returns>
    public async Task<BotSavedMessageEntity> SaveMessageFromUpdate(long botId, BotChatEntity? chat, BotUserEntity? user, Message message)
    {
        if (message == null) throw new ArgumentNullException(nameof(message));

        BotSavedMessageEntity savedMessageEntity = new()
        {
            BotId = botId,
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

    /// <summary>
    /// Проверяет, есть ли сохранённое сообщение с определённым типом медиа.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="telegramChatId">ID чата</param>
    /// <param name="telegramUserId">ID пользователя</param>
    /// <param name="mediaGroupId">ID группы медиа</param>
    /// <returns>true, если есть такое сообщение</returns>
    public async Task<bool> HasSavedMessageWithMediaType(long botId, long? telegramChatId, long? telegramUserId, string? mediaGroupId)
    {
        return 0 < ( await _db.SavedMessages.CountAsync(sm => sm.BotId == botId &&
                                                              (telegramChatId == null || sm.TelegramChatId == telegramChatId)  &&
                                                              (telegramUserId == null || sm.TelegramUserId == telegramUserId) &&
                                                              sm.MediaGroupId == mediaGroupId));
    }
    
}