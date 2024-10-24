using Microsoft.EntityFrameworkCore;
using MultipleTestBot.Db.AppDb.Entities;

namespace MultipleTestBot.Db.AppDb;

public partial class AppDbContext : DbContext
{
    private const string appSchema = "app";


    protected string? InitiatorUserId { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options, IServiceProvider serviceProvider) : base(options)
    {
    }
    
    // Коллекции данных
    public DbSet<PostEntity> Posts => Set<PostEntity>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        SetSchemasToTables(builder);
        SetAllToSnakeCase(builder);
        SetFilters(builder);
        ConfigureEntities(builder);
        base.OnModelCreating(builder);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var e in
                 ChangeTracker.Entries<IBaseEntity>())
        {
            switch (e.State)
            {
                case EntityState.Added:
                    e.Entity.CreatedAt = DateTimeOffset.Now;
                    e.Entity.CreatedBy = InitiatorUserId;
                    break;
                case EntityState.Modified:
                    e.Entity.UpdatedAt = DateTimeOffset.Now;
                    e.Entity.UpdatedBy = InitiatorUserId;
                    break;
                case EntityState.Deleted:
                    e.Entity.DeletedAt = DateTimeOffset.Now;
                    e.Entity.DeletedBy = InitiatorUserId;
                    e.State = EntityState.Modified;
                    break;
            }
        }

        return base.SaveChangesAsync(ct);
    }
    
}
