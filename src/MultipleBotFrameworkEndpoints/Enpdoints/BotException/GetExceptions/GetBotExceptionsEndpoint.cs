using FastEndpoints;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Enpdoints.Chat;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotException.GetExceptions;

public class GetBotExceptionsRequest : Pagination, IOrdered
{
    public List<long>? BotIds { get; set; }
    public List<long>? Ids { get; set; }
    public string? Order { get; set; }
}

public class GetBotExceptionsEndpoint : Endpoint<GetBotExceptionsRequest, PagedList<BotExceptionEntity>>
{
    private BotDbContext _db;

    public GetBotExceptionsEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/get");
        AllowAnonymous();
        Group<ExceptionGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем список ошибок в приложении.";
            s.Description = "Необходимо чтобы отслеживать работу приложения, делать своевременные доработки.";
        });
    }

    public override async Task HandleAsync(GetBotExceptionsRequest r, CancellationToken c)
    {
        var query = _db.Exceptions
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), e => e.BotId != null && r.BotIds.Contains(e.BotId.Value))
            .WhereIf(r.Ids is not null && r.Ids.Any(), e => r.Ids.Contains(e.Id))
            .Order(r.Order);

        var result = await PagedList<BotExceptionEntity>.ToPagedListAsync(query, r);
        await SendAsync(result);
    }
}