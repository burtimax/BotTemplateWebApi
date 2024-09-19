using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

public class RenewBotRequest
{
    public long Id { get; set; }
}

public class RenewBotEndpoint : Endpoint<RenewBotRequest, BotEntity>
{
    private IBotsManagerService _botsManagerService;

    public RenewBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    public override void Configure()
    {
        Post("/renew");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Перезапускаем бота.";
            s.Description = "Перезагружаем бота, обновляем данные вебхука, добавляем бота в botsManager (кеш).";
        });
    }

    public override async Task HandleAsync(RenewBotRequest r, CancellationToken c)
    {
        var bot = await _botsManagerService.RenewBotInfo(r.Id);
        await SendAsync(bot);
    }
}

