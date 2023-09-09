using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BotFramework.Models;
using Telegram.Bot.Types;

namespace BotFramework.Db.Entity
{
    /// <summary>
    /// Сущность чата
    /// </summary>
    public class BotChat : BaseBotEntity<long>
    {
        /// <summary>
        /// Идентификатор чата в телеграм
        /// </summary>
        public long? TelegramId { get; set; }

        /// <summary>
        /// Идентификатор чата в телеграм
        /// </summary>
        /// <remarks>Некоторые чаты вместо long идентификатора имеют username идентификатор</remarks>
        public string? TelegramUsername { get; set; }

        /// <summary>
        /// Внешний ключ на пользователя
        /// </summary>
        public long BotUserId { get; set; }
        public BotUser BotUser { get; set; }

        /// <summary>
        /// Состояние чата 
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        private List<string> _states = new ();

        /// <summary>
        /// Хранилище данных чата.
        /// </summary>
        /// <remarks>
        /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
        /// </remarks>
        private Dictionary<string, string> _dataDatabaseDictionary = new ();
    
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
        public ChatId ChatId {
            get
            {
                return TelegramId != null ? 
                    new ChatId((long)TelegramId) : 
                    new ChatId(TelegramUsername ?? "");
            }
        }
        
        #endregion
    }
}