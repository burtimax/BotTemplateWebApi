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
    /// Получить реферальную кампанию по коду.
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    public Task<ReferralCampaign?> GetCampaignByCode(long botId, string? code);

    /// <summary>
    /// Создать реферальную кампанию для пользователя.
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="userTelegramId"></param>
    /// <param name="name">Название кампании.</param>
    /// <returns></returns>
    public Task<ReferralCampaign> CreateCampaign(long botId, long userTelegramId, string name);
    
    /// <summary>
    /// Получить участника с реферальными кампаниями.
    /// </summary>
    /// <param name="botId"></param>
    /// <param name="userTelegramId"></param>
    /// <returns></returns>
    public Task<ReferralParticipant> GetParticipantWithCampaigns(long botId, long userTelegramId);
}