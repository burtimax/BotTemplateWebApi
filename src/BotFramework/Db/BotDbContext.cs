using BotFramework.Db.Entity;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Db
{
    public class BotDbContext : DbContext
    {
        public BotDbContext()
        {
            
        }
        
        public DbSet<BotUser> Users { get; set; }
        public DbSet<BotChat> Chats { get; set; }
        public DbSet<BotMessage> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}