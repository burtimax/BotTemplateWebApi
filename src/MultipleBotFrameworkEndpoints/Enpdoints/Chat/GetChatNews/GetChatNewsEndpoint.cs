using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using MultipleTestBot.Endpoints.Bot;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.GetChats;

public class GetChatNewsRequest
{
    public List<long> ChatIds { get; set; }
}

public class GetChatNewsResponseItem
{
    public long Id { get; set; }
    public long BotId { get; set; }
    public long TelegramId { get; set; }
    public long CountNews { get; set; }
}

public class GetChatNewsEndpoint : Endpoint<GetChatNewsRequest, List<GetChatNewsResponseItem>>
{
    private BotDbContext _db;

    public GetChatNewsEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/news");
        AllowAnonymous();
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Получение кол-ва непрочитанных модератором сообщений в чатах.";
            s.Description = "Возвращает кол-во непрочитанных сообщений в чатах.";
        });
    }

    public override async Task HandleAsync(GetChatNewsRequest r, CancellationToken c)
    {
        List<BotChatEntity> chats = await _db.Chats
            .Where(ch => r.ChatIds.Contains(ch.Id))
            .ToListAsync();

        List<long> chatTelegramIds = chats.Select(ch => ch.TelegramId).ToList();

        var data = await _db.ChatHistory
            .Where(h => h.IsViewed == false)
            .GroupBy(h => new{h.BotId, h.TelegramChatId})
            .Select(h => new
            {
                TelegramId = h.Key.TelegramChatId,
                BotId = h.Key.BotId,
                Count = h.LongCount()
            })
            .ToListAsync();

        List<GetChatNewsResponseItem> result = new();

        foreach (var chat in chats)
        {
            var d = data.FirstOrDefault(i => i.TelegramId == chat.TelegramId && i.BotId == chat.BotId);
            
            result.Add(new()
            {
                Id = chat.Id, 
                TelegramId = chat.TelegramId, 
                BotId = d?.BotId ?? -1, 
                CountNews = d?.Count ?? 0,
            });
        }
        
        await SendAsync(result);
    }
}