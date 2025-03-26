using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework.Db.BroadcastDb;

public class BroadcastDbContextFactory
{
    public BroadcastDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BroadcastDbContext>();
        optionsBuilder.UseNpgsql("Data Source=blog.db");

        return new BroadcastDbContext(optionsBuilder.Options);
    }
    
}