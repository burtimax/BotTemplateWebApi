using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Models;

namespace MultipleBotFramework.Db.ReferralDb.Entity;

public class ReferralParticipant : BaseEntity<long>
{
    public long BotId { get; set; }
    
    /// <summary>
    /// Телеграм ИД пользователя.
    /// </summary>
    public long UserTelegramId { get; set; }
    
    /// <summary>
    /// По какой ссылки пришел пользователь.
    /// </summary>
    public string? ReferrerCode { get; set; }

    public List<ReferralCampaign> Campaigns { get; set; } = new();
    
    /// <summary>
    /// Хранилище данных чата.
    /// </summary>
    /// <remarks>
    /// Не переименовывать свойство, потому что оно в модели БД <seealso cref="BotDbContext.OnModelCreating"/>
    /// </remarks>
    private Dictionary<string, string> _dataDictionary = new ();
    
    private ComplexDictionary? _chatData = null;

    /// <summary>
    /// Свойство для работы с временными данными чата.
    /// </summary>
    [NotMapped] public ComplexDictionary Data =>  _chatData ??= new ComplexDictionary(_dataDictionary);
    
}