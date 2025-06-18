using System.Collections.Generic;
using MultipleBotFramework.Enums;

namespace MultipleBotFramework.Db.Entity;

public class BotEntity : BaseEntity<long>
{
    public string Token { get; set; }
    public string? Username { get; set; }
    public string? Name { get; set; }
    public string? ShortDescription { get; set; }
    public string? Description { get; set; }
    public string? Comment { get; set; }
    
    /// <summary>
    /// Адрес вебхука для бота.
    /// </summary>
    public string? Webhook { get; set; }

    /// <summary>
    /// Статус бота.
    /// </summary>
    public BotStatus Status { get; set; } = BotStatus.On;

    /// <summary>
    /// Навигационное поле.
    /// Пользователи бота.
    /// </summary>
    public List<BotUserEntity> Users { get; set; }
}