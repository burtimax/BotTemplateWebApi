using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Enpdoints.User;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

sealed class BlockUserRequest 
{
    public List<long>? TelegramIds { get; set; }
    public List<long>? Ids { get; set; }
    public List<long>? BotIds { get; set; }
    public bool IsBlocked { get; set; }
}

sealed class BlockUserEndpoint : Endpoint<BlockUserRequest, List<BotUserEntity>>
{
    private BotDbContext _db;

    public BlockUserEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/block");
        AllowAnonymous();
        Group<UserGroup>();
    }

    public override async Task HandleAsync(BlockUserRequest r, CancellationToken c)
    {
        if ((r.Ids is null || r.Ids.Any() == false)
            && (r.TelegramIds is null || r.TelegramIds.Any() == false))
            throw new Exception($"{nameof(r.TelegramIds)} или {nameof(r.Ids)} не должны быть пустые!");
        
        List<BotUserEntity>? users = await _db.Users
            .WhereIf(r.TelegramIds is not null && r.TelegramIds.Any(), u => r.TelegramIds.Contains(u.TelegramId))
            .WhereIf(r.Ids is not null && r.Ids.Any(), u => r.Ids.Contains(u.Id))
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), u => r.BotIds!.Contains(u.BotId))
            .ToListAsync();

        if (users is null || users.Any() == false)
        {
            throw new Exception("Пользователи не найдены");
            return;
        }

        foreach (var user in users)
        {
            user.IsBlocked = r.IsBlocked;
        }
        await _db.SaveChangesAsync();
        
        await SendAsync(users);
    }
}