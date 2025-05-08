using System.Threading.Tasks;

namespace MultipleBotFramework.Services.Referral;

public interface IReferralCodeService
{
    public Task<string> GetUniqueReferralCodeAsync();
}