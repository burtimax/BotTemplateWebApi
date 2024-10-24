using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

public class UpdateBotRequest
{
    public long Id { get; set; }
    public string? Token { get; set; }
    
    [DefaultValue(null)]
    public string? Comment { get; set; }
    
    [DefaultValue(null)]
    public BotStatus? Status { get; set; }
}

public class UpdateBotEndpoint : Endpoint<UpdateBotRequest, BotEntity>
{
    private IBotsManagerService _botsManagerService;
    

    public UpdateBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    public override void Configure()
    {
        Patch("/update");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Обновляем данные бота (токен, статус и прочее).";
            s.Description = "Используем когда у бота обновляем токен. Можем поменять статус бота, включить его (1), отключить его (0) или отправить на тех. работы (2).";
        });
    }

    public override async Task HandleAsync(UpdateBotRequest r, CancellationToken c)
    {
        var bot = await _botsManagerService.UpdateBot(r.Id, r.Token, r.Comment, r.Status);
        await SendAsync(bot);
    }
}

