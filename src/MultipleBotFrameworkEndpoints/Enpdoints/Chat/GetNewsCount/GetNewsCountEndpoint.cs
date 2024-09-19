using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using MultipleTestBot.Endpoints.Bot;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.GetChats;

public class GetNewsCountRequest
{
    public List<long>? BotIds { get; set; }
}

public class GetNewsCountResponse
{
    public long CountChatNews { get; set; }
}

public class GetNewsCountEndpoint : Endpoint<GetNewsCountRequest, GetNewsCountResponse>
{
    private BotDbContext _db;

    public GetNewsCountEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/news-count");
        AllowAnonymous();
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Кол-во непрочитанных чатов.";
            s.Description = "Показывает кол-во чатов, в которых есть непрочитанные сообщения.";
        });
    }

    public override async Task HandleAsync(GetNewsCountRequest r, CancellationToken c)
    {
        var count = await _db.ChatHistory
            .Where(h => h.IsViewed == false)
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), h => r.BotIds!.Contains(h.BotId))
            .GroupBy(h => new {h.BotId, h.TelegramChatId})
            .Select(h => new
            {
                TelegramId = h.Key.TelegramChatId,
                BotId = h.Key.BotId,
                Count = h.LongCount()
            })
            .LongCountAsync();
        
        await SendAsync(new ()
        {
            CountChatNews = count,
        });
    }
}