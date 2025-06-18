using System;
using System.Threading.Tasks;
using MultipleBotFramework.Db.BroadcastDb.Entity;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Services.Interfaces;

public interface IBotNotificationService
{
    public Task AddNotification(BotNotification notification);
    internal Task<BotNotificationService.SendNotificationResult> SendNextNotification();
    public Task AddNotification(long botId, long chatId, long savedMessageId, DateTimeOffset? sendAt = null, string? type = null, string? key = null);

    public Task AddNotification(long botId, long chatId, long fromChatId, int fromMessageId,
        ReplyMarkup? replyMarkup = null, DateTimeOffset? sendAt = null, string? type = null, string? key = null);

    public Task AddNotification(long botId, long chatId, string text, string? photoFileId = null,
        ReplyMarkup? replyMarkup = null, DateTimeOffset? sendAt = null, string? type = null, string? key = null);

}