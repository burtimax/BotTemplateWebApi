using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework.Db.ReferralDb;

public class ReferralDbContextFactory
{
    public ReferralDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ReferralDbContext>();
        optionsBuilder.UseNpgsql("Data Source=blog.db");

        return new ReferralDbContext(optionsBuilder.Options);
    }
    
}