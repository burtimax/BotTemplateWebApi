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

/// <summary>
/// Сервис для управления и отправки уведомлений ботом.
/// </summary>
public class BotNotificationService : IBotNotificationService
{
    private readonly BroadcastDbContext _db;
    private readonly BotDbContext _botDb;

    /// <summary>
    /// Конструктор сервиса уведомлений.
    /// </summary>
    /// <param name="db">Контекст базы данных рассылок</param>
    /// <param name="botDb">Контекст основной базы данных бота</param>
    public BotNotificationService(BroadcastDbContext db, BotDbContext botDb)
    {
        _db = db;
        _botDb = botDb;
    }

    /// <summary>
    /// Добавляет уведомление для отправки сохранённого сообщения.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="chatId">ID чата для отправки</param>
    /// <param name="savedMessageId">ID сохранённого сообщения</param>
    /// <param name="type">Тип уведомления</param>
    public async Task AddNotification(long botId, long chatId, long savedMessageId, 
        DateTimeOffset? sendAt = null, string? type = null, string? key = null)
    {
        BotNotification notification = new()
        {
            BotId = botId,
            BotToken = await GetBotToken(botId),
            SendToChatId = chatId,
            SendAt = sendAt ?? DateTimeOffset.MinValue,
            SavedMessageId = savedMessageId,
            Type = type,
            Key = key,
            Status = BotNotificationStatus.New
        };
        await AddNotification(notification);
        return;
    }
    
    /// <summary>
    /// Добавляет уведомление для копирования сообщения из другого чата.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="chatId">ID чата для отправки</param>
    /// <param name="fromChatId">ID исходного чата</param>
    /// <param name="fromMessageId">ID исходного сообщения</param>
    /// <param name="replyMarkup">Клавиатура для сообщения</param>
    /// <param name="type">Тип уведомления</param>
    public async Task AddNotification(long botId, long chatId, long fromChatId, int fromMessageId, ReplyMarkup? replyMarkup = null, 
        DateTimeOffset? sendAt = null, string? type = null, string? key = null)
    {
        BotNotification notification = new()
        {
            BotId = botId,
            BotToken = await GetBotToken(botId),
            SendToChatId = chatId,
            SendAt = sendAt ?? DateTimeOffset.MinValue,
            FromChatId = fromChatId,
            FromMessageId = fromMessageId,
            Type = type,
            Key = key,
            Status = BotNotificationStatus.New
        };
        await AddNotification(notification);
        return;
    }
    
    /// <summary>
    /// Добавляет уведомление с текстом и (опционально) фото.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="chatId">ID чата для отправки</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="photoFileId">ID файла фото</param>
    /// <param name="replyMarkup">Клавиатура для сообщения</param>
    /// <param name="type">Тип уведомления</param>
    public async Task AddNotification(long botId, long chatId, string text, string? photoFileId = null, ReplyMarkup? replyMarkup = null, 
        DateTimeOffset? sendAt = null, string? type = null, string? key = null)
    {
        BotNotification notification = new()
        {
            BotId = botId,
            BotToken = await GetBotToken(botId),
            SendToChatId = chatId,
            SendAt = sendAt ?? DateTimeOffset.MinValue,
            Text = text,
            PhotoFileId = photoFileId,
            ReplyMarkupJson = replyMarkup?.ToJson() ?? null,
            Type = type,
            Key = key,
            Status = BotNotificationStatus.New
        };
        await AddNotification(notification);
        return;
    }

    /// <summary>
    /// Получает токен бота по его ID.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <returns>Токен бота</returns>
    private async Task<string> GetBotToken(long botId)
    {
        var bot = await _botDb.Bots.FirstAsync(b => b.Id == botId);
        return bot.Token;
    }
    
    /// <summary>
    /// Добавляет уведомление в базу данных.
    /// </summary>
    /// <param name="notification">Уведомление</param>
    public async Task AddNotification(BotNotification notification)
    {
        _db.BotNotifications.Add(notification);
        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Получает следующее новое уведомление для отправки.
    /// </summary>
    /// <returns>Следующее уведомление или null</returns>
    public async Task<BotNotification?> GetNextNotification()
    {
        return await _db.BotNotifications.Where(n => n.Status == BotNotificationStatus.New
            && n.SendAt <= DateTimeOffset.Now)
            .OrderBy(n => n.Id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Отправляет следующее уведомление из очереди.
    /// </summary>
    /// <returns>Результат отправки</returns>
    /// <exception cref="Exception">В случае внутренних ошибок</exception>
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

        try
        {
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

    /// <summary>
    /// Возможные результаты отправки уведомления.
    /// </summary>
    public enum SendNotificationResult
    {
        Nothing = 0,
        InternalException = 1,
        Failed = 3,
        Success = 4,
    }
}