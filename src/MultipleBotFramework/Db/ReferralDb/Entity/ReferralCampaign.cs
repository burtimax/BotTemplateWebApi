using MultipleBotFramework.Db.Entity;

namespace MultipleBotFramework.Db.ReferralDb.Entity;

public class ReferralCampaign : BaseEntity<long>
{
    /// <summary>
    /// НЕ МЕНЯТЬ ЗНАЧЕНИЕ!!!
    /// </summary>
    public const string DefaultName = "_default_";
    
    public long BotId { get; set; }
    public long ParticipantId { get; set; }
    public string Name { get; set; } = DefaultName;
    public ReferralParticipant? Participant { get; set; }

    public string Code { get; set; }
    public long ReferralCount { get; set; }
}