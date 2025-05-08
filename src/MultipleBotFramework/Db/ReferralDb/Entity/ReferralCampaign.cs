using MultipleBotFramework.Db.Entity;

namespace MultipleBotFramework.Db.ReferralDb.Entity;

public class ReferralCampaign : BaseEntity<long>
{
    public const string DefaultName = "Referral";
    
    public long BotId { get; set; }
    public long ParticipantId { get; set; }
    public string Name { get; set; } = DefaultName;
    public ReferralParticipant? Participant { get; set; }

    public string Code { get; set; }
    public long ReferralCount { get; set; }
}