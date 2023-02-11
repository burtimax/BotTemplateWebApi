using System.ComponentModel.DataAnnotations;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Пользователь
    /// </summary>
    
    public class BotUser : BaseBotEntity<long>
    {
        /// <summary>
        /// Telegram идентификатор пользователя
        /// </summary>
        public long TelegramId { get; set; }
        
        /// <summary>
        /// Ник в Telegram (eg @username)
        /// </summary>
        public string? TelegramUsername { get; set; }

        /// <summary>
        /// Телефон
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Роль пользователя (user, moderator, admin)
        /// </summary>
        [MaxLength(20)]
        public string Role { get; set; }
        
        /// <summary>
        /// Статус пользователя (активный, заблокированный)
        /// </summary>
        public string? Status { get; set; }
        
        /// <summary>
        /// Имя пользователя в Telegram
        /// </summary>
        public string? TelegramFirstname { get; set; }
        
        /// <summary>
        /// Фамилия пользователя в Telegram
        /// </summary>
        public string? TelegramLastname { get; set; }
    }
}