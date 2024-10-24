using System;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Базовая модель сущности (базовые поля).
    /// </summary>
    public class BaseBotEntity<T> : IBaseBotEntityWithoutIdentity
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