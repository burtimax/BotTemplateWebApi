namespace MultipleBotFramework_Obsolete.Db.Entity;

public class BotOwnerEntity : BaseEntity<long>
{
    public long BotId { get; set; }
    public long UserTelegramId { get; set; }

    public string? Username { get; set; }
    
    public string? TelegramFirstname { get; set; }
    
    public string? TelegramLastname { get; set; }
}