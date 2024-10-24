using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotFramework.Repository;

/// <inheritdoc />
public class BotUpdateRepository : IBotUpdateRepository
{
    private readonly BotDbContext _db;
    
    public BotUpdateRepository(BotDbContext db)
    {
        _db = db;
    }
    
    /// <inheritdoc />
    public Task<List<BotUpdate>> GetLastCountChatMessages(long chatEntityId, int count)
    {
        IQueryable<BotUpdate> query = _db.Updates
            .Where(m => m.BotChatId == chatEntityId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count);

        return query.ToListAsync();
    }
}