using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db.ReferralDb.Entity;
using MultipleBotFramework.Extensions;

namespace MultipleBotFramework.Db.ReferralDb;

public class ReferralDbContextConfiguration
{
    private const string schema = "referral";
    
    internal static void ConfigureContext(ModelBuilder builder)
    {
        SetTableAndSchema(builder);
        SetIndexes(builder);
        SetOtherConfigs(builder);
        SetBaseConfiguration(builder);
    }

    public static void SetBaseConfiguration(ModelBuilder builder)
    {
        builder.SetFilters();
        builder.SetAllToSnakeCase();
    }

    private static void SetTableAndSchema(ModelBuilder builder)
    {
        builder.Entity<ReferralParticipant>().ToTable("participants", schema);
        builder.Entity<ReferralCampaign>().ToTable("campaigns", schema);
    }
    
    private static void SetOtherConfigs(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ReferralParticipant>(entity =>
        {
            entity.Property("_dataDictionary");
        });
    }

    private static void SetIndexes(ModelBuilder modelBuilder)
    {
        
    }
}