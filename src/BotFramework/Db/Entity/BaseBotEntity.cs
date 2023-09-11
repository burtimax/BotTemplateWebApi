using System;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Базовая модель сущности (базовые поля).
    /// </summary>
    public class BaseBotEntity<T>
    {
        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        [Comment("Идентификатор сущности.")]
        public T Id { get; set; }
        
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
}