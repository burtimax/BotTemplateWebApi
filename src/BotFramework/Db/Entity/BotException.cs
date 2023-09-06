using System;

namespace BotFramework.Db.Entity;

public class BotException : BaseBotEntity<long>
{
    public long? UserId { get; set; }
    public BotUser? User { get; set; }

    public long? ChatId { get; set; }
    public BotChat? Chat { get; set; }
    
    public Guid? UpdateId { get; set; }
    public BotUpdate? Update { get; set; }

    public string? ExceptionMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? ReportDescription { get; set; }
    public string? ReportFileName { get; set; }
}