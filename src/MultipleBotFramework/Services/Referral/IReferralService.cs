using System.Threading.Tasks;
using MultipleBotFramework.Db.ReferralDb.Entity;

namespace MultipleBotFramework.Services.Referral;

public interface IReferralService
{
    /// <summary>
    /// Обработать реферала.
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="userTelegramId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<ReferralProgramStatus> HandleReferralCode(long botId, long userTelegramId, string? code);

    /// <summary>
    /// Получить участника с реферальными кампаниями.
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="userTelegramId"></param>
    /// <returns></returns>
    public Task<ReferralParticipant> GetParticipantWithCampaigns(long botId, long userTelegramId);
}