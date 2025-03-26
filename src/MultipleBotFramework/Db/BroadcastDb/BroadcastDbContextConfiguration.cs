using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db.BroadcastDb.Entity;
using MultipleBotFramework.Extensions;

namespace MultipleBotFramework.Db.BroadcastDb;

public class BroadcastDbContextConfiguration
{
    private const string schema = "broadcast";
    
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
        builder.Entity<BroadcastTask>().ToTable("broadcast_tasks", schema);
        builder.Entity<BroadcastMessage>().ToTable("broadcast_messages", schema);
    }
    
    private static void SetOtherConfigs(ModelBuilder modelBuilder)
    {
        
    }

    private static void SetIndexes(ModelBuilder modelBuilder)
    {
        
    }
}