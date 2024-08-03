using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Models;

namespace MultipleBotFramework.Db.Entity
{
    /// <summary>
    /// Пользователь.
    /// </summary
    [Comment("Таблица пользователей бота.")]
    public class BotUserEntity : BaseEntity<long>
    {
        /// <summary>
        /// Telegram идентификатор пользователя.
        /// </summary>
        [Comment("Идентификатор пользователя в Telegram.")]
        public long TelegramId { get; set; }
        
        /// <summary>
        /// Ник в Telegram.
        /// </summary>
        [Comment("Ник пользователя в Telegram.")]
        public string? TelegramUsername { get; set; }

        /// <summary>
        /// Телефон.
        /// </summary>
        [MaxLength(20)]
        [Comment("Номер телефона пользователя.")]
        public string? Phone { get; set; }

        /// <summary>
        /// Роль пользователя (user, moderator, admin).
        /// </summary>
        [MaxLength(20)]
        [Comment("Роль пользователя в боте. Например [user, moderator, admin].")]
        public string Role { get; set; }
        
        /// <summary>
        /// Необязательно. <a href="https://en.wikipedia.org/wiki/IETF_language_tag">IETF language tag</a>
        /// Языковой код пользователя.
        /// </summary>
        [Comment("Код языка пользователя. Ссылка на коды [https://en.wikipedia.org/wiki/IETF_language_tag]")]
        public string? LanguageCode { get; set; }

        /// <summary>
        /// Флаг заблокированного пользователя.
        /// </summary>
        [Comment("Флаг заблокированного пользователя.")]
        public bool IsBlocked { get; set; } = false;
        
        /// <summary>
        /// Имя пользователя в Telegram.
        /// </summary>
        [Comment("Имя пользователя в Telegram.")]
        public string? TelegramFirstname { get; set; }
        
        /// <summary>
        /// Фамилия пользователя в Telegram.
        /// </summary>
        [Comment("Фамилия пользователя в Telegram.")]
        public string? TelegramLastname { get; set; }
        
        /// <summary>
        /// Словарь для хранения свойств пользователя (динамически).
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        [Comment("Словарь для хранения свойств пользователя (динамически).")]
        private Dictionary<string, string> _propertiesDatabaseDictionary = new ();

        /// <summary>
        /// Внешний ключ на бота.
        /// </summary>
        [Comment("Внешний ключ на бота.")]
        public long BotId { get; set; }
        public BotEntity? Bot { get; set; }
        
        #region NotMappedData
        
        private ComplexDictionary? _additionalProperties = null;

        /// <summary>
        /// Свойство для работы c динамическими полями пользователя.
        /// </summary>
        /// <remarks>
        /// Для того, чтобы динамически можно было расширять информацию по пользователю.
        /// </remarks>
        [NotMapped] public ComplexDictionary AdditionalProperties => _additionalProperties ??= new ComplexDictionary(_propertiesDatabaseDictionary);

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