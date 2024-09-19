using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

public class DeleteBotRequest
{
    public long Id { get; set; }
}

public class DeleteBotEndpoint : Endpoint<DeleteBotRequest>
{
    private IBotsManagerService _botsManagerService;
    

    public DeleteBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    public override void Configure()
    {
        Delete("/delete");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Удаляем бота из приложения. Бот перестает работать в приложении.";
            s.Description = "SoftDelete сущности бота. Бот перестанет работать.";
        });
    }

    public override async Task HandleAsync(DeleteBotRequest r, CancellationToken c)
    {
        await _botsManagerService.DeleteBot(r.Id);
        await SendOkAsync();
    }
}

