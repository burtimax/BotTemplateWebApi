using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Db.ReferralDb;
using MultipleBotFramework.Db.ReferralDb.Entity;
using MultipleBotFramework.Repository;

namespace MultipleBotFramework.Services.Referral;

public class ReferralService : IReferralService
{
    protected readonly ReferralDbContext _db;
    protected readonly IBaseBotRepository _baseBotRepository;
    protected readonly IReferralCodeService _referralCodeService;
    
    public ReferralService(ReferralDbContext db, IBaseBotRepository baseBotRepository, IReferralCodeService referralCodeService)
    {
        _db = db;
        _baseBotRepository = baseBotRepository;
        _referralCodeService = referralCodeService;
    }
    
    public async Task<ReferralProgramStatus> HandleReferralCode(long botId, long userTelegramId, string? code)
    {
        var participant = await _db.Participants.FirstOrDefaultAsync(x => x.BotId == botId 
                                                                          && x.UserTelegramId == userTelegramId && x.ReferrerCode != null);

        if (participant != null)
        {
            return ReferralProgramStatus.AlreadyUsed;
        }
        
        var user = await _baseBotRepository.GetUserByIdentity(botId, userTelegramId.ToString());
        if (user == null) return ReferralProgramStatus.NotFound;

        ReferralParticipant newParticipant = new ReferralParticipant()
        {
            BotId = botId,
            UserTelegramId = userTelegramId,
            ReferrerCode = code,
        };
        
        _db.Participants.Add(newParticipant);
        await _db.SaveChangesAsync();

        var campaign = await GetCampaignByCode(botId, code);
        if (campaign is not null)
        {
            campaign.ReferralCount += 1;
            _db.Campaigns.Update(campaign);
            await _db.SaveChangesAsync();
        }
        
        return ReferralProgramStatus.Succeeded;
    }

    public async Task<ReferralCampaign?> GetCampaignByCode(long botId, string? code)
    {
        return await _db.Campaigns
            .Include(c => c.Participant)
            .FirstOrDefaultAsync(x => x.BotId == botId && x.Code == code);
    }

    public async Task<ReferralCampaign> CreateCampaign(long botId, long userTelegramId, string name)
    {
        var participant = await GetParticipantWithCampaigns(botId, userTelegramId);
        
        ReferralCampaign campaign =
            new ReferralCampaign()
            {
                BotId = botId,
                Code = await _referralCodeService.GetUniqueReferralCodeAsync(),
                ParticipantId = participant.Id,
                Name = name,
            };

        _db.Campaigns.Add(campaign);
        await _db.SaveChangesAsync();
        return campaign;
    }
    
    public async Task<ReferralParticipant> GetParticipantWithCampaigns(long botId, long userTelegramId)
    {
        ReferralParticipant? participant = await _db.Participants
            .Include(p => p.Campaigns)
            .FirstOrDefaultAsync(x => x.BotId == botId && x.UserTelegramId == userTelegramId);

        // Если у пользователя нет реферальных кампаний, создаем ему по умолчанию.
        if (participant == null)
        {
            participant = new ReferralParticipant()
            {
                BotId = botId,
                UserTelegramId = userTelegramId,
            };
            _db.Participants.Add(participant);
            await _db.SaveChangesAsync();
        };

        if (participant.Campaigns.Any(c => c.Name == ReferralCampaign.DefaultName) == false)
        {
            ReferralCampaign campaign =
                new ReferralCampaign()
                {
                    BotId = botId,
                    Code = await _referralCodeService.GetUniqueReferralCodeAsync(),
                    ParticipantId = participant.Id,
                    Name = ReferralCampaign.DefaultName,
                };

            _db.Campaigns.Add(campaign);
            await _db.SaveChangesAsync();
        }
        
        return participant;
    }
    
}