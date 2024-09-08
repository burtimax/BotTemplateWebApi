using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using MultipleBotFramework.Extensions;
using MultipleTestBot.Db.AppDb.Entities;

namespace MultipleTestBot.Db.AppDb;

public partial class AppDbContext
{
    /// <summary>
    /// Таблицы, свойства, ключи, внеш. ключи, индексы переводит в нижний регистр в БД.
    /// </summary>
    protected void SetAllToSnakeCase(ModelBuilder builder)
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

    /// <summary>
    /// Задать наименование таблиц и схемы для таблиц.
    /// </summary>
    private void SetSchemasToTables(ModelBuilder builder)
    {
        // Определение сущностей по схемам.
        
        builder.Entity<PostEntity>().ToTable("posts", AppDbContext.appSchema);
        
    }
    
    /// <summary>
    /// Настройка фильтров запросов.
    /// </summary>
    public static void SetFilters(ModelBuilder modelBuilder)
    {
        var entities = modelBuilder.Model
            .GetEntityTypes()
            .Where(e => e.ClrType.BaseType == typeof(BaseEntity<Guid>))
            .Select(e => e.ClrType);
        
        Expression<Func<BaseEntity<Guid>, bool>> 
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

    public static void ConfigureEntities(ModelBuilder builder)
    {
        // var jsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.General);
        //
        // builder.Entity<SOME_ENTITY>(entity =>
        // {
        //     entity
        //         .Property(x => x.ContentItem)
        //         .HasColumnName("PROPERTY")
        //         .HasColumnType("JSONB")
        //         .HasConversion(
        //             ctp => JsonSerializer.Serialize(ctp, jsonSerializerOptions),
        //             cfp => JsonSerializer.Deserialize<PROPERTY_TYPE>(cfp, jsonSerializerOptions),
        //             ValueComparer.CreateDefault(typeof(PROPERTY_TYPE), true)
        //         );
        // });
    }
}