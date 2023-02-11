using System;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Базовая сущность
    /// </summary>
    public class BaseBotEntity<T>
    {
        public T Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public DateTimeOffset DeletedAt { get; set; }
    }
}