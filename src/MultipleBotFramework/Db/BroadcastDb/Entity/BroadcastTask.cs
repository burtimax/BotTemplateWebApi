using System;
using System.Collections.Generic;
using System.ComponentModel;
using MultipleBotFramework.Db.Entity;

namespace MultipleBotFramework.Db.BroadcastDb.Entity;

public class BroadcastTask : BaseEntity<long>
{
    public string BotToken { get; set; }
    public long BotId { get; set; }
    public long FromChatId { get; set; }
    public int FromMessageId { get; set; }
    public string? MediaFileId { get; set; }
    public string? Text { get; set; }
    public string? ReplyMarkupJson { get; set; }
    public BroadcastTaskStatus Status { get; set; }
    /// <summary>
    /// Когда можно начать рассылку.
    /// </summary>
    public DateTimeOffset StartAt { get; set; }
    
    public List<BroadcastMessage>? Messages { get; set; }
}

public enum BroadcastTaskStatus
{
    None = 0,
    New = 1,
    InProcess = 2,
    Terminated = 3,
    Done = 4
}