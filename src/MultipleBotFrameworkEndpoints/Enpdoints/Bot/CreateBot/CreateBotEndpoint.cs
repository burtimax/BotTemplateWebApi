using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

public class CreateBotRequest
{
    public string Token { get; set; }
    
    [DefaultValue(null)]
    public string? Comment { get; set; }
    
    [DefaultValue(null)]
    public BotStatus? Status { get; set; }
}

public class CreateBotEndpoint : Endpoint<CreateBotRequest, BotEntity>
{
    private IBotsManagerService _botsManagerService;
    

    public CreateBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    public override void Configure()
    {
        Post("/create");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Зарегистрировать (добавить) бота в приложение";
            s.Description = "Добавляем бота в приложение, бот начинает работать в приложении по логике приложения.";
        });
    }

    public override async Task HandleAsync(CreateBotRequest r, CancellationToken c)
    {
        var bot = await _botsManagerService.CreateBot(r.Token, r.Comment, r.Status);
        await SendAsync(bot);
    }
}

