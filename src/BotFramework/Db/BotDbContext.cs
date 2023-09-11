using BotFramework.Db.Entity;
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
    }
}