using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db.Entity;

/// <summary>
/// Таблица сопостовлений пользователей и разрешений.
/// </summary>
[Comment("Таблица сопоставлений пользователей и разрешений.")]
public class BotUserClaim : BaseBotEntity<long>
{
    [Comment("Внешний ключ на пользователя.")]
    public long UserId { get; set; }
    public virtual BotUser User { get; set; }

    [Comment("Внешний ключ разрешения.")]
    public long ClaimId { get; set; }
    public virtual BotClaim Claim { get; set; }
}