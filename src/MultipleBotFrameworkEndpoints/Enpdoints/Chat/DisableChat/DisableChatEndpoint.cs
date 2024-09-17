using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.DisableChat;

public class DisableChatRequest
{
    public long Id { get; set; }
    public long DisableForSeconds { get; set; }
}

public class DisableChatEndpoint : Endpoint<DisableChatRequest, BotChatEntity>
{
    private BotDbContext _db;

    public DisableChatEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/disable");
        AllowAnonymous();
        Group<ChatGroup>();
    }

    public override async Task HandleAsync(DisableChatRequest r, CancellationToken c)
    {
        var chat = await _db.Chats
            .FirstOrDefaultAsync(c => c.Id == r.Id);

        if (chat is null) throw new Exception($"Не найден чат [Id = {r.Id}]");

        if (r.DisableForSeconds <= 0) chat.DisabledUntil = null;
        
        chat.DisabledUntil = DateTimeOffset.Now.AddSeconds(r.DisableForSeconds);

        await _db.SaveChangesAsync();
        
        await SendAsync(chat);
    }
}