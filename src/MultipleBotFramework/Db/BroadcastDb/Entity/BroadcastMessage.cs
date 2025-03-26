using MultipleBotFramework.Db.Entity;

namespace MultipleBotFramework.Db.BroadcastDb.Entity;

public class BroadcastMessage : BaseEntity<long>
{
    public long ChatId { get; set; }
    public bool IsSuccess { get; set; }

    public long BroadcastTaskId { get; set; }
    public BroadcastTask? BroadcastTask { get; set; }

    public string? ErrorLog { get; set; }
}