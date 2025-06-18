using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Db.ReferralDb.Entity;
using MultipleBotFramework.Services.Referral;

namespace MultipleBotFramework.Db.ReferralDb
{
    public class ReferralDbContext : DbContext
    {
        public ReferralDbContext(){}
        
        public ReferralDbContext(DbContextOptions<ReferralDbContext> options) : base(options){}

        public DbSet<ReferralParticipant> Participants { get; set; }
        public DbSet<ReferralCampaign> Campaigns { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     //Определение провайдера необходимо для создания миграции, поэтому пусть пока побудет здесь.
        //      string mockString = "Host=127.0.0.1;Port=5432;Database=test_bot_db;Username=postgres;Password=123";
        //      optionsBuilder.UseNpgsql(mockString);
        //     base.OnConfiguring(optionsBuilder);
        // }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ReferralDbContextConfiguration.ConfigureContext(modelBuilder);
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