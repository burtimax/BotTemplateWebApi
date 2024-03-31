using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Repository;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(){}
        
        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options){}

        public DbSet<BotUser> Users { get; set; }
        public DbSet<BotChat> Chats { get; set; }
        public DbSet<BotUpdate> Updates { get; set; }
        public DbSet<BotClaim> Claims { get; set; }
        public DbSet<BotUserClaim> UserClaims { get; set; }
        public DbSet<BotException> Exceptions { get; set; }
        public DbSet<BotSavedMessage> SavedMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Определение провайдера необходимо для создания миграции, поэтому пусть пока побудет здесь.
            string mockString = "Host=127.0.0.1;Port=5432;Database=test_bot_db;Username=postgres;Password=123";
            optionsBuilder.UseNpgsql(mockString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BotDbContextConfiguration.ConfigureContext(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
        
        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            foreach (var e in
                     ChangeTracker.Entries<IBaseBotEntityWithoutIdentity>())
            {
                switch (e.State)
                {
                    case EntityState.Added:
                        e.Entity.CreatedAt = DateTimeOffset.Now;
                        break;
                    case EntityState.Modified:
                        e.Entity.UpdatedAt = DateTimeOffset.Now;
                        break;
                    case EntityState.Deleted:
                        e.Entity.DeletedAt = DateTimeOffset.Now;
                        e.State = EntityState.Modified;
                        break;
                }
            }

            return base.SaveChangesAsync(ct);
        }
    }
    
}