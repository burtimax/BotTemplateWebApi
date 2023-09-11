using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db.Entity;

/// <summary>
/// Разрешения.
/// </summary>
[Comment("Таблица разрешений.")]
public class BotClaim : BaseBotEntity<long>
{
    /// <summary>
    /// Имя разрешения.
    /// </summary>
    [Comment("Имя разрешения.")]
    public string Name { get; set; }

    /// <summary>
    /// Описание разрешения.
    /// </summary>
    [Comment("Описание разрешения.")]
    public string? Description { get; set; }
}