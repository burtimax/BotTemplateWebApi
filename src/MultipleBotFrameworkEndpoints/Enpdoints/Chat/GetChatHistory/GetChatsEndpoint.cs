using FastEndpoints;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.GetChatHistory;

public class GetChatHistoryRequest : Pagination, IOrdered
{
    public List<long>? BotIds { get; set; }
    public List<long>? ChatTelegramIds { get; set; }
    public string? Order { get; set; }
}

public class GetChatHistoryEndpoint : Endpoint<GetChatHistoryRequest, PagedList<BotChatHistoryEntity>>
{
    private BotDbContext _db;

    public GetChatHistoryEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/history/get");
        AllowAnonymous();
        Group<ChatGroup>();
    }

    public override async Task HandleAsync(GetChatHistoryRequest r, CancellationToken c)
    {
        var query = _db.ChatHistory
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), b => r.BotIds.Contains(b.BotId))
            .WhereIf(r.ChatTelegramIds is not null && r.ChatTelegramIds.Any(), b => r.ChatTelegramIds.Contains(b.TelegramChatId))
            .Order(r.Order);

        if (string.IsNullOrEmpty(r.Order))
        {
            query.OrderByDescending(i => i.CreatedAt);
        }
        
        var result = await PagedList<BotChatHistoryEntity>.ToPagedListAsync(query, r);
        await SendAsync(result);
    }
}