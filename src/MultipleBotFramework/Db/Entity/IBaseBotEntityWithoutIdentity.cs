using System;
using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework.Db.Entity;

public interface IBaseBotEntityWithoutIdentity
{
    /// <summary>
    /// Дата и время создания сущности в БД.
    /// </summary>
    [Comment("Дата и время создания сущности в БД.")]
    public DateTimeOffset CreatedAt { get; set; }
        
    /// <summary>
    /// Дата и время последнего обновления сущности в БД.
    /// </summary>
    [Comment("Дата и время последнего обновления сущности в БД.")]
    public DateTimeOffset? UpdatedAt { get; set; }
        
    /// <summary>
    /// Дата и время удаления сущности в БД.
    /// </summary>
    [Comment("Дата и время удаления сущности в БД.")]
    public DateTimeOffset? DeletedAt { get; set; }
}