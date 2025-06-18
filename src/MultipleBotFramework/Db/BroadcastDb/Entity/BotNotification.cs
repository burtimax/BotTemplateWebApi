using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MultipleBotFramework.Db.Entity;

namespace MultipleBotFramework.Db.BroadcastDb.Entity;

public class BotNotification : BaseEntity<long>
{
    public string BotToken { get; set; }
    public long BotId { get; set; }
    /// <summary>
    /// Кому отправить уведомление.
    /// </summary>
    public long SendToChatId { get; set; }
    /// <summary>
    /// Тип уведомления.
    /// </summary>
    public string? Type { get; set; }
    /// <summary>
    /// Ключ уведомления (альтернативный идентификатор).
    /// </summary>
    public string? Key { get; set; }
    /// <summary>
    /// Если это пересылаемое сообщение.
    /// </summary>
    public long? FromChatId { get; set; }
    /// <summary>
    /// Если это пересылаемое сообщение.
    /// </summary>
    public int? FromMessageId { get; set; }
    /// <summary>
    /// Отрпавить сохраненное сообщение.
    /// </summary>
    public long? SavedMessageId { get; set; }
    public string? PhotoFileId { get; set; }
    public string? Text { get; set; }
    public string? ReplyMarkupJson { get; set; }
    /// <summary>
    /// Ошибки при отправке уведомлений.
    /// </summary>
    public string? ErrorLog { get; set; }
    public BotNotificationStatus Status { get; set; }
    
    /// <summary>
    /// Когда нужно отправить уведомление?
    /// </summary>
    public DateTimeOffset SendAt { get; set; }
}

public enum BotNotificationStatus
{
    None = 0,
    New = 1,
    Fail = 2,
    Success = 3
}