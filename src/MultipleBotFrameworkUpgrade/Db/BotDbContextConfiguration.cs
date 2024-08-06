using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Extensions;

namespace MultipleBotFrameworkUpgrade.Db;

public class BotDbContextConfiguration
{
    private const string schema = "bot";
    
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
        builder.Entity<BotEntity>().ToTable("bots", schema);
        builder.Entity<BotOwnerEntity>().ToTable("bot_owners", schema);
        builder.Entity<BotUserEntity>().ToTable("users", schema);
        builder.Entity<BotChatEntity>().ToTable("chats", schema);
        builder.Entity<BotUpdateEntity>().ToTable("updates", schema);
        builder.Entity<BotClaimEntity>().ToTable("claims", schema);
        builder.Entity<BotUserClaimEntity>().ToTable("user_claims", schema);
        builder.Entity<BotExceptionEntity>().ToTable("exceptions", schema);
        builder.Entity<BotSavedMessageEntity>().ToTable("saved_messages", schema);
    }
    
    /// <summary>
    /// Сделать SnakeCase для всех сущностей, полей, внешних ключей, индексов и прочее...)
    /// </summary>
    /// <param name="builder"></param>
    // private static void SetAllTableNamesToSnakeCase(ModelBuilder builder)
    // {
    //     foreach (var entityType in builder.Model.GetEntityTypes())
    //     {
    //         entityType.SetTableName(entityType.GetTableName().ToSnakeCase());
    //
    //         foreach (var property in entityType.GetProperties())
    //         {
    //             var schema = entityType.GetSchema();
    //             var tableName = entityType.GetTableName();
    //             var storeObjectIdentifier = StoreObjectIdentifier.Table(tableName, schema);
    //             property.SetColumnName(property.GetColumnName(storeObjectIdentifier).ToSnakeCase());
    //         }
    //
    //         foreach (var key in entityType.GetKeys())
    //             key.SetName(key.GetName().ToSnakeCase());
    //
    //         foreach (var key in entityType.GetForeignKeys())
    //             key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
    //
    //         foreach (var index in entityType.GetIndexes())
    //             index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
    //     }
    // }
    
    private static void SetOtherConfigs(ModelBuilder modelBuilder)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
        
        // Приватное свойство добавляем в модель.
        // <see href="https://learn.microsoft.com/ru-ru/ef/core/modeling/backing-field?tabs=data-annotations">
        modelBuilder.Entity<BotChatEntity>(entity =>
        {
            entity.Property("_dataDatabaseDictionary");
        });

        // Хранение свойств пользователя.
        modelBuilder.Entity<BotUserEntity>(entity =>
        {
            entity.Property("_propertiesDatabaseDictionary");
        });
        
        modelBuilder.Entity<BotChatEntity>()
            .Property("_states");

        // Ограничение Unique на разрешения пользователя. 
        modelBuilder.Entity<BotUserClaimEntity>()
            .HasIndex(uc => new { uc.UserId, uc.ClaimId })
            .IsUnique();
    }

    private static void SetIndexes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BotUserEntity>()
            .HasIndex(u => u.TelegramId);

        modelBuilder.Entity<BotUpdateEntity>()
            .HasIndex(u => u.ChatId);
    }
}