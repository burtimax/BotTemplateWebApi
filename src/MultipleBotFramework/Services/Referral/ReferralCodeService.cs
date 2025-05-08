using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db.ReferralDb;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Services.Referral;

public class ReferralCodeService : IReferralCodeService
{
    private const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string DefaultPromocode = "WUaqUQnGoq";

    private readonly ReferralDbContext _db;

    public ReferralCodeService(ReferralDbContext db)
    {
        _db = db;
    }

    private string GenerateReferralCode(int refCodeLength = 8)
    {
        var timestamp = DateTime.UtcNow.Ticks.GetHashCode();
        var random = new Random(timestamp);
        var chars = new char[refCodeLength];

        for (var i = 0; i < chars.Length; i++)
        {
            chars[i] = AllowedChars[random.Next(AllowedChars.Length)];
        }

        return new string(chars);
    }

    public async Task<string> GetUniqueReferralCodeAsync()
    {
        var counter = 0;

        while (counter != 20)
        {
            var referralCode = GenerateReferralCode();

            var isRefCodeExist = await _db.Campaigns.AnyAsync(c => c.Code == referralCode);

            if (isRefCodeExist)
            {
                counter++;
                continue;
            }

            return referralCode;
        }

        throw new Exception("Cannot generate unique referral code.");
    }
}