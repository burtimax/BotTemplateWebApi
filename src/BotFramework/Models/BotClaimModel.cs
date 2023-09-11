using BotFramework.Db.Entity;

namespace BotFramework.Models;

/// <summary>
/// Модель разрешения пользователя.
/// Модель для сущности <see cref="BotClaim"/>.
/// </summary>
public class BotClaimModel
{
    /// ИД разрешения.
    public long Id { get; set; }
    
    /// <summary>
    /// Наименование разрешения.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Описание разрешения.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="id">ИД.</param>
    /// <param name="name">Наименование.</param>
    /// <param name="description">Описание.</param>
    public BotClaimModel(int id, string name, string? description = null)
    {
        Id = id;
        Name = name;
        Description = description;
    }

    /// <summary>
    /// Перегрузка неявного преобразование в сущность разрешения.
    /// <see cref="BotClaim"/>.
    /// </summary>
    public static implicit operator BotClaim(BotClaimModel botClaim)
    {
        return new BotClaim()
        {
            Id = botClaim.Id,
            Name = botClaim.Name,
            Description = botClaim.Description
        };
    }
    
}