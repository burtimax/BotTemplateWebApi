using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Db.BroadcastDb.Entity;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Extensions.ITelegramApiClient;
using MultipleBotFramework.Models;
using MultipleBotFramework.Services.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Services;

public class BotNotificationService : IBotNotificationService
{
    private readonly BroadcastDbContext _db;
    private readonly BotDbContext _botDb;

    public BotNotificationService(BroadcastDbContext db, BotDbContext botDb)
    {
        _db = db;
        _botDb = botDb;
    }

    public async Task AddNotification(long botId, long chatId, long savedMessageId, string? type = null)
    {
        BotNotification notification = new()
        {
            BotId = botId,
            BotToken = await GetBotToken(botId),
            SendToChatId = chatId,
            SavedMessageId = savedMessageId,
            Type = type,
            Status = BotNotificationStatus.New
        };
        await AddNotification(notification);
        return;
    }
    
    public async Task AddNotification(long botId, long chatId, long fromChatId, int fromMessageId, ReplyMarkup? replyMarkup = null, string? type = null)
    {
        BotNotification notification = new()
        {
            BotId = botId,
            BotToken = await GetBotToken(botId),
            SendToChatId = chatId,
            FromChatId = fromChatId,
            FromMessageId = fromMessageId,
            Type = type,
            Status = BotNotificationStatus.New
        };
        await AddNotification(notification);
        return;
    }
    
    public async Task AddNotification(long botId, long chatId, string text, string? photoFileId = null, ReplyMarkup? replyMarkup = null, string? type = null)
    {
        BotNotification notification = new()
        {
            BotId = botId,
            BotToken = await GetBotToken(botId),
            SendToChatId = chatId,
            Text = text,
            PhotoFileId = photoFileId,
            ReplyMarkupJson = replyMarkup?.ToJson() ?? null,
            Type = type,
            Status = BotNotificationStatus.New
        };
        await AddNotification(notification);
        return;
    }

    private async Task<string> GetBotToken(long botId)
    {
        var bot = await _botDb.Bots.FirstAsync(b => b.Id == botId);
        return bot.Token;
    }
    
    public async Task AddNotification(BotNotification notification)
    {
        _db.BotNotifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    public async Task<BotNotification?> GetNextNotification()
    {
        return await _db.BotNotifications.Where(n => n.Status == BotNotificationStatus.New)
            .OrderBy(n => n.Id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Используется для отправки следующего в очереди уведомления.
    /// </summary>
    /// <remarks>
    /// Этот метод использует <see cref="BroadcastNotificationJob"/>
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<SendNotificationResult> SendNextNotification()
    {
        bool success = false;
        bool internalException = false;
        string errorLog = "";
        
        BotNotification? n = await GetNextNotification();
        if(n == null) return SendNotificationResult.Nothing;

        ReplyMarkup? replyMarkup = null;
        if (string.IsNullOrEmpty(n.ReplyMarkupJson) == false)
            replyMarkup = n.ReplyMarkupJson.FromJson<ReplyMarkup>();
        
        TelegramBotClient client = new(n.BotToken);
        
        var chat = await _botDb.Chats.FirstOrDefaultAsync(c =>
            c.BotId == n.BotId && c.TelegramId == n.SendToChatId);

        if (chat == null)
        {
            internalException = true;
            throw new Exception($"Не найден чат [{n.SendToChatId}]");
        }

        if (chat.Status == BotChatStatus.Banned || chat.Status == BotChatStatus.Left)
        {
            internalException = true;
            throw new Exception($"Чат [{chat.TelegramId}] заблокирован для отправки уведомлений");
        }

        try
        {
            if (n.SavedMessageId.HasValue)
            {
                await client.SendSavedMessage(n.SendToChatId, _botDb, n.SavedMessageId.Value,
                    replyMarkup: replyMarkup ?? null);
                success = true;
            }

            if (!success && n.FromChatId.HasValue)
            {
                if (n.FromMessageId == null)
                {
                    internalException = true;
                    throw new Exception($"Обязательное поле для уведомления [{nameof(n.FromMessageId)}].");
                }
                    

                await client.CopyMessageAsync(n.SendToChatId, n.FromChatId.Value, n.FromMessageId.Value,
                    replyMarkup: replyMarkup);
                success = true;
            }

            if (!success && string.IsNullOrEmpty(n.Text) == false)
            {
                if (string.IsNullOrEmpty(n.PhotoFileId) == false)
                {
                    await client.SendPhotoAsync(n.SendToChatId, n.PhotoFileId, replyMarkup: replyMarkup);
                    success = true;
                }
                else
                {
                    await client.SendMessageAsync(n.SendToChatId, n.Text, replyMarkup: replyMarkup);
                    success = true;
                }
            }

            if (!success)
            {
                internalException = true;
                throw new Exception($"Не подобрали способ отправки уведомления");
            }
        }
        catch (Exception e)
        {
            errorLog = e.Message;
        }
        finally
        {
            n.ErrorLog = errorLog;
            n.Status = string.IsNullOrEmpty(n.ErrorLog) ? BotNotificationStatus.Success : BotNotificationStatus.Fail;
            await _db.SaveChangesAsync();
        }

        if (internalException) return SendNotificationResult.InternalException;
        return success ? SendNotificationResult.Success : SendNotificationResult.Failed;
    }

    public enum SendNotificationResult
    {
        Nothing = 0,
        InternalException = 1,
        Failed = 3,
        Success = 4,
    }
}