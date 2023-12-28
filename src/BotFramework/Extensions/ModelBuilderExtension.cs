using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BotFramework.Extensions;

public static class ModelBuilderExtension
{
    /// <summary>
    /// Таблицы, свойства, ключи, внеш. ключи, индексы переводит в нижний регистр в БД.
    /// </summary>
    /// <param name="builder"></param>
    public static void SetAllToSnakeCase(this ModelBuilder builder)
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
}