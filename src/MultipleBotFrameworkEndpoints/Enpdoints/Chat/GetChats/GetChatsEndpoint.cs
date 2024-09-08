﻿using FastEndpoints;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using MultipleTestBot.Endpoints.Bot;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.GetChats;

public class GetChatsRequest : Pagination, IOrdered
{
    public List<long>? BotIds { get; set; }
    public List<long>? Ids { get; set; }
    public string? Order { get; set; }
}

public class GetChatsEndpoint : Endpoint<GetChatsRequest, PagedList<BotChatEntity>>
{
    private BotDbContext _db;

    public GetChatsEndpoint(BotDbContext db)
    {
        _db = db;
    }

    public override void Configure()
    {
        Get("/get");
        AllowAnonymous();
        Group<ChatGroup>();
    }

    public override async Task HandleAsync(GetChatsRequest r, CancellationToken c)
    {
        var query = _db.Chats
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), b => r.BotIds.Contains(b.BotId))
            .WhereIf(r.Ids is not null && r.Ids.Any(), b => r.Ids.Contains(b.Id))
            .Order(r.Order);

        var result = await PagedList<BotChatEntity>.ToPagedListAsync(query, r);
        await SendAsync(result);
    }
}