using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using MultipleTestBot.Endpoints.Bot;

public class GetBotsRequest : Pagination, IOrdered
{
    public List<long>? Ids { get; set; }
    public string? Order { get; set; }
}

public class GetBotsEndpoint : Endpoint<GetBotsRequest, PagedList<BotEntity>>
{
    private BotDbContext _db;

    public GetBotsEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/get");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем список ботов.";
            s.Description = "Сущности ботов. Можем фильтровать, пагинировать, сортировать.";
        });
    }

    public override async Task HandleAsync(GetBotsRequest r, CancellationToken c)
    {
        var query = _db.Bots
            .WhereIf(r.Ids is not null && r.Ids.Any(), b => r.Ids.Contains(b.Id))
            .Order(r.Order);

        var bots = await PagedList<BotEntity>.ToPagedListAsync(query, r);
        await SendAsync(bots);
    }
}

