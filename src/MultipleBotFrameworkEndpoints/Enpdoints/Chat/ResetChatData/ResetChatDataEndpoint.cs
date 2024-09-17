using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Models;
using MultipleBotFrameworkEndpoints.Enpdoints.Chat.DisableChat;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.ResetChatData;

public class ResetChatDataRequest
{
    public long Id { get; set; }
}

public class ResetChatDataEndpoint : Endpoint<ResetChatDataRequest, BotChatEntity>
{
    private BotDbContext _db;

    public ResetChatDataEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Post("/reset-data");
        AllowAnonymous();
        Group<ChatGroup>();
    }

    public override async Task HandleAsync(ResetChatDataRequest r, CancellationToken c)
    {
        var chat = await _db.Chats
            .FirstOrDefaultAsync(c => c.Id == r.Id);

        if (chat is null) throw new Exception($"Не найден чат [Id = {r.Id}]");

        string defaultState = BotConstants.StartState;
        chat.States.Set(defaultState, ChatStateSetterType.SetRoot);
        chat.Data.Clear();
        _db.Chats.Update(chat);
        await _db.SaveChangesAsync();
        
        await SendAsync(chat);
    }
}