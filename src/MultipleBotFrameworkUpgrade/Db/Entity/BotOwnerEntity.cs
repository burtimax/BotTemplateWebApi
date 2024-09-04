namespace MultipleBotFrameworkUpgrade.Db.Entity;

public class BotOwnerEntity : BaseEntity<long>
{
    /// <summary>
    /// Внешний ключ на таблицу ботов.
    /// </summary>
    public long BotId { get; set; }
    public BotEntity Bot { get; set; }
    
    /// <summary>
    /// Telegram ID владельца.
    /// </summary>
    public long UserTelegramId { get; set; }

    /// <summary>
    /// Username владельца.
    /// </summary>
    public string? Username { get; set; }
    
    /// <summary>
    /// Имя владельца бота.
    /// </summary>
    public string? TelegramFirstname { get; set; }
    
    /// <summary>
    /// Фамилия владельца бота.
    /// </summary>
    public string? TelegramLastname { get; set; }
}