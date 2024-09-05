using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework_Obsolete.Db.Entity;

/// <summary>
/// Таблица сопостовлений пользователей и разрешений.
/// </summary>
[Comment("Таблица сопоставлений пользователей и разрешений.")]
public class BotUserClaimEntity : BaseEntity<long>
{
    [Comment("Внешний ключ на пользователя.")]
    public long UserId { get; set; }
    public virtual BotUserEntity User { get; set; }

    [Comment("Внешний ключ разрешения.")]
    public long ClaimId { get; set; }
    public virtual BotClaimEntity Claim { get; set; }
    
    /// <summary>
    /// Внешний ключ на бота.
    /// </summary>
    [Comment("Внешний ключ на бота.")]
    public long BotId { get; set; }
    public BotEntity? Bot { get; set; }
}