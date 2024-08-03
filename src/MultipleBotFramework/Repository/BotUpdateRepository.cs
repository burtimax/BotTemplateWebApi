using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;

namespace MultipleBotFramework.Repository;

/// <inheritdoc />
public class BotUpdateRepository : IBotUpdateRepository
{
    private readonly BotDbContext _db;
    
    public BotUpdateRepository(BotDbContext db)
    {
        _db = db;
    }
    
    /// <inheritdoc />
    public Task<List<BotUpdateEntity>> GetLastCountChatMessages(long botId, long chatEntityId, int count)
    {
        IQueryable<BotUpdateEntity> query = _db.Updates
            .Where(m => m.BotId == botId && m.ChatId == chatEntityId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(count);

        return query.ToListAsync();
    }
}