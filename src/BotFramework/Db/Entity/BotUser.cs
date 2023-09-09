using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BotFramework.Models;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Пользователь.
    /// </summary
    public class BotUser : BaseBotEntity<long>
    {
        /// <summary>
        /// Telegram идентификатор пользователя.
        /// </summary>
        public long TelegramId { get; set; }
        
        /// <summary>
        /// Ник в Telegram.
        /// </summary>
        public string? TelegramUsername { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        [MaxLength(20)]
        public string? Phone { get; set; }

        /// <summary>
        /// Роль пользователя (user, moderator, admin).
        /// </summary>
        [MaxLength(20)]
        public string Role { get; set; }
        
        /// <summary>
        /// Статус пользователя (активный, заблокированный).
        /// </summary>
        public string? Status { get; set; }
        
        /// <summary>
        /// Имя пользователя в Telegram.
        /// </summary>
        public string? TelegramFirstname { get; set; }
        
        /// <summary>
        /// Фамилия пользователя в Telegram.
        /// </summary>
        public string? TelegramLastname { get; set; }

        /// <summary>
        /// Словарь для хранения свойств пользователя (динамически).
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        private Dictionary<string, string> _propertiesDatabaseDictionary = new ();
        
        /// <summary>
        /// Словарь для хранения разрешений пользователя (динамически).
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        private Dictionary<string, string> _claimsDatabaseDictionary = new ();

        #region NotMappedData
        
        private ComplexDictionary? _optionalProperties = null;

        /// <summary>
        /// Свойство для работы c динамическими полями пользователя.
        /// </summary>
        /// <remarks>
        /// Для того, чтобы динамически можно было расширять информацию по пользователю.
        /// </remarks>
        [NotMapped] public ComplexDictionary OptionalProperties => _optionalProperties ??= new ComplexDictionary(_propertiesDatabaseDictionary);
        
        private ComplexDictionary? _claims = null;
        
        /// <summary>
        /// Свойство для работы c динамическими разрешениями пользователя.
        /// </summary>
        /// <remarks>
        /// Для того, чтобы динамически можно было управлять разрешениями пользователя.
        /// </remarks>
        [NotMapped] public ComplexDictionary Claims => _optionalProperties ??= new ComplexDictionary(_claimsDatabaseDictionary);
        
        #endregion
        
        /// <summary>
        /// Получить строку Username в виде [@username].
        /// </summary>
        /// <returns></returns>
        public string? GetUsernameLink()
        {
            return string.IsNullOrEmpty(this.TelegramUsername) == false
                ? $"@{this.TelegramUsername}"
                : null;
        }
    }
}