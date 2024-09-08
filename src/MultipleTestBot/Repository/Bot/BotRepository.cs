using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleTestBot.Extensions;
using MultipleTestBot.Models;
using MultipleTestBot.Repository.Bot.Dto;

namespace MultipleTestBot.Repository.Bot;

/// <inheritdoc />
public class BotRepository : IBotRepository
{
    private readonly BotDbContext _db;
    
    public BotRepository(BotDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<PagedList<BotEntity>> GetBots(GetBotsDto dto)
    {
        var query = _db.Bots
            .WhereIf(dto.Ids is not null && dto.Ids.Any(), b => dto.Ids.Contains(b.Id))
            .Order(dto.Order);

        return PagedList<BotEntity>.ToPagedListAsync(query, dto);
    }
}