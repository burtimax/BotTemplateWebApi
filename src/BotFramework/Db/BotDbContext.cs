using BotFramework.Db.Entity;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db
{
    public class BotDbContext : DbContext
    {
        
        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options){}

        public DbSet<BotUser> Users { get; set; }
        public DbSet<BotChat> Chats { get; set; }
        public DbSet<BotMessage> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Приватное свойство добавляем в модель.
            // <see href="https://learn.microsoft.com/ru-ru/ef/core/modeling/backing-field?tabs=data-annotations">
            modelBuilder.Entity<BotChat>()
                .Property("_data");
            modelBuilder.Entity<BotChat>()
                .Property("_states");
            
            base.OnModelCreating(modelBuilder);
        }
    }
}