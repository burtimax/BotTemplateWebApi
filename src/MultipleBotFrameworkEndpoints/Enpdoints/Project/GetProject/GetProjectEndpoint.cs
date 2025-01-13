using System.ComponentModel;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Extensions.ITelegramApiClient;
using MultipleBotFramework.Models;
using MultipleBotFramework.Options;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;
using MultipleTestBot.Endpoints.Bot;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

public class GetProjectRequest
{
    public string Key { get; set; }
}

public class GetProjectResponse
{
    public string Name { get; set; }
    public string Token { get; set; }
}


public class GetProjectEndpoint : Endpoint<GetProjectRequest, GetProjectResponse>
{
    private readonly BotConfiguration _config;

    public GetProjectEndpoint(IOptions<BotConfiguration> config)
    {
        _config = config.Value;
    }

    public override void Configure()
    {
        Post("/get");
        AllowAnonymous();
        Group<ProjectGroup>();
        Summary(s =>
        {
            s.Summary = "Получить информацию по проекту";
            s.Description = "Получить информацию по проекту";
        });
    }

    public override async Task HandleAsync(GetProjectRequest r, CancellationToken c)
    {
        if (string.IsNullOrEmpty(r.Key))
        {
            await SendErrorsAsync();
            return;
        }
        
        await SendAsync(new GetProjectResponse()
        {
            Name = _config.Name ?? $"Проект {new Random().Next().ToString()}",
            Token = "BEARER TOKEN"
        });
    }
}

