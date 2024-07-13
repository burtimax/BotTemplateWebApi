using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db.Entity;

/// <summary>
/// Сущность бота.
/// </summary>
[Comment("Сущность бота.")]
public class BotEntity : BaseEntity<long>
{
    /// <summary>
    /// Токен бота.
    /// </summary>
    [Comment("Токен бота.")]
    public string Token { get; set; }
    
    /// <summary>
    /// Ник бота.
    /// @telegram_bot
    /// </summary>
    [Comment("Ник бота [@telegram_bot].")]
    public string BotUsername { get; set; }
    
    /// <summary>
    /// Наименование бота.
    /// </summary>
    [Comment("Наименование бота.")]
    public string? Name { get; set; }

    /// <summary>
    /// Описание бота.
    /// </summary>
    [Comment("Описание к боту.")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Комментарий к боту.
    /// </summary>
    [Comment("Комментарий к боту.")]
    public string? Comment { get; set; }
}