using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using BotFramework.Db.Entity;
using BotFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;

namespace BotFramework.Db;

public class BotDbContextConfiguration
{
    private const string schema = "bot";
    
    public static void ConfigureContext(ModelBuilder builder)
    {
        SetTableAndSchema(builder);
        SetFilters(builder);
        SetOtherConfigs(builder);
        SetAllTableNamesToSnakeCase(builder);
    }

    private static void SetTableAndSchema(ModelBuilder builder)
    {
        builder.Entity<BotUser>().ToTable("users", schema);
        builder.Entity<BotChat>().ToTable("chats", schema);
        builder.Entity<BotUpdate>().ToTable("updates", schema);
    }
    
    /// <summary>
    /// Сделать SnakeCase для всех сущностей, полей, внешних ключей, индексов и прочее...)
    /// </summary>
    /// <param name="builder"></param>
    private static void SetAllTableNamesToSnakeCase(ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.GetTableName().ToSnakeCase());

            foreach (var property in entityType.GetProperties())
            {
                var schema = entityType.GetSchema();
                var tableName = entityType.GetTableName();
                var storeObjectIdentifier = StoreObjectIdentifier.Table(tableName, schema);
                property.SetColumnName(property.GetColumnName(storeObjectIdentifier).ToSnakeCase());
            }

            foreach (var key in entityType.GetKeys())
                key.SetName(key.GetName().ToSnakeCase());

            foreach (var key in entityType.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());

            foreach (var index in entityType.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
        }
    }
    
    private static void SetFilters(ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model
            .GetEntityTypes()
            .Where(e => e.ClrType.BaseType == typeof(BaseBotEntity<long>))
            .Select(e => e.ClrType);

        Expression<Func<BaseBotEntity<long>, bool>> 
            expression = del => del.DeletedAt == null;

        foreach (var e in entities)
        {
            ParameterExpression p = Expression.Parameter(e);
            Expression body =
                ReplacingExpressionVisitor
                    .Replace(expression.Parameters.Single(),
                        p, expression.Body);

            modelBuilder.Entity(e)
                .HasQueryFilter(
                    Expression.Lambda(body, p));
        }
    }
    
    private static void SetOtherConfigs(ModelBuilder modelBuilder)
    {
        var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
        
        // Приватное свойство добавляем в модель.
        // <see href="https://learn.microsoft.com/ru-ru/ef/core/modeling/backing-field?tabs=data-annotations">
        modelBuilder.Entity<BotChat>(entity =>
        {
            entity.Property("_data");
        });
        
        modelBuilder.Entity<BotChat>()
            .Property("_states");   
    }
    
}