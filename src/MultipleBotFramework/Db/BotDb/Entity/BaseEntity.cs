using System;
using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework.Db.Entity
{
    /// <summary>
    /// Базовая модель сущности (базовые поля).
    /// </summary>
    public class BaseEntity<T> : IBaseBotEntityWithoutIdentity
    {
        /// <summary>
        /// Идентификатор сущности.
        /// </summary>
        [Comment("Идентификатор сущности.")]
        public T Id { get; set; }
        
        /// <inheritdoc />
        public DateTimeOffset CreatedAt { get; set; }
        
        /// <inheritdoc />
        public DateTimeOffset? UpdatedAt { get; set; }
        
        /// <inheritdoc />
        public DateTimeOffset? DeletedAt { get; set; }
    }
}