using FastEndpoints;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Enpdoints.User;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

sealed class GetUsersRequest : Pagination, IOrdered
{
    public List<long>? Ids { get; set; }
    public List<long>? BotIds { get; set; }
    public string? Order { get; set; }
}

sealed class GetUsersEndpoint : Endpoint<GetUsersRequest, PagedList<BotUserEntity>>
{
    private BotDbContext _db;

    public GetUsersEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/get");
        AllowAnonymous();
        Group<UserGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем список пользователей.";
            s.Description = "Можем отфильтровать сущности пользователей по ботам, по ИД, пагинировать и сотрировать.";
        });
    }

    public override async Task HandleAsync(GetUsersRequest r, CancellationToken c)
    {
        var query = _db.Users
            .WhereIf(r.Ids is not null && r.Ids.Any(), b => r.Ids!.Contains(b.Id))
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), b => r.BotIds!.Contains(b.BotId))
            .Order(r.Order);

        var users = await PagedList<BotUserEntity>.ToPagedListAsync(query, r);
        await SendAsync(users);
    }
}