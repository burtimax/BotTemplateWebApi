namespace MultipleBotFrameworkUpgrade.Db.Entity;

public class BotEntity : BaseEntity<long>
{
    public string Token { get; set; }
    public string? Username { get; set; }

    public string? Description { get; set; }
    
    public string? Comment { get; set; }
}