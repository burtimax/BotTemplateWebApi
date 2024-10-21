using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Models;
using Telegram.BotAPI;

namespace MultipleBotFramework.Db.Entity
{
    /// <summary>
    /// Сущность чата.
    /// </summary>
    [Comment("Сущность чата.")]
    public class BotChatEntity : BaseEntity<long>
    {
        /// <summary>
        /// Идентификатор чата в телеграм.
        /// </summary>
        [Comment("Идентификатор чата в Telegram.")]
        public long TelegramId { get; set; }

        /// <summary>
        /// private, channel, group и прочие.
        /// </summary>
        public string? Type { get; set; }
        
        /// <summary>
        /// Идентификатор чата в телеграм.
        /// </summary>
        /// <remarks>Некоторые чаты вместо long идентификатора имеют username идентификатор</remarks>
        [Comment("Строковый идентификатор чата в телеграм. Некоторые чаты вместо long идентификатора имеют username идентификатор.")]
        public string? TelegramUsername { get; set; }
        
        [Comment("Заголовок чата")]
        public string? Title { get; set; }

        /// <summary>
        /// Внешний ключ на пользователя.
        /// </summary>
        [Comment("Внешний ключ на пользователя.")]
        public long? BotUserId { get; set; }
        public BotUserEntity? BotUser { get; set; }

        /// <summary>
        /// Состояние чата.
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        [Comment("Состояние чата.")]
        private List<string> _states = new ();

        /// <summary>
        /// Хранилище данных чата.
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        private Dictionary<string, string> _dataDatabaseDictionary = new ();

        /// <summary>
        /// Внешний ключ на бота.
        /// </summary>
        [Comment("Внешний ключ на бота.")]
        public long BotId { get; set; }
        public BotEntity? Bot { get; set; }

        /// <summary>
        /// Бот не отвечает/не реагирует чату до определенного времени.
        /// </summary>
        [Comment("Бот не отвечает/не реагирует чату до определенного времени.")]
        public DateTimeOffset? DisabledUntil { get; set; }
        
        #region NotMappedData
        
        private ComplexDictionary? _chatData = null;

        /// <summary>
        /// Свойство для работы с временными данными чата.
        /// </summary>
        [NotMapped] public ComplexDictionary Data =>  _chatData ??= new ComplexDictionary(_dataDatabaseDictionary);

        private ChatStates? _chatStates;
        
        /// <summary>
        /// Свойство для работы с состояниями чата.
        /// </summary>
        [NotMapped] public ChatStates States => _chatStates ??= new ChatStates(_states);


        /// <summary>
        /// Получить Telegram идентификатор чата (Id или Username)
        /// </summary>
        /// <remarks>NotMapped свойство</remarks>
        [NotMapped]
        public long ChatId => TelegramId;

        /// <summary>
        /// Получить тип чата.
        /// </summary>
        /// <returns></returns>
        public ChatType GetType()
        {
            return this.Type switch
            {
                ChatTypes.Sender => ChatType.Sender,
                ChatTypes.Private => ChatType.Private,
                ChatTypes.Group => ChatType.Group,
                ChatTypes.Supergroup => ChatType.Supergroup,
                ChatTypes.Channel => ChatType.Channel,
                _ => ChatType.Unknown
            };
        }
        
        #endregion
    }
}