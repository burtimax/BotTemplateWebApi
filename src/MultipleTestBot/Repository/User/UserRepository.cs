using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleTestBot.Extensions;
using MultipleTestBot.Models;
using MultipleTestBot.Repository.User.Dto;

namespace MultipleTestBot.Repository.User;

/// <inheritdoc />
public class UserRepository : IUserRepository
{
    private readonly BotDbContext _db;

    public UserRepository(BotDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public Task<PagedList<BotUserEntity>> GetUsers(GetUsersDto dto)
    {
        var query = _db.Users
            .WhereIf(dto.Ids is not null && dto.Ids.Any(), b => dto.Ids!.Contains(b.Id))
            .WhereIf(dto.BotIds is not null && dto.BotIds.Any(), b => dto.BotIds!.Contains(b.BotId))
            .Order(dto.Order);

        return PagedList<BotUserEntity>.ToPagedListAsync(query, dto);
    }
}