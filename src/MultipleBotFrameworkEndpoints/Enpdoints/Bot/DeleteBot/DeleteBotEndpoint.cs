/// <summary>
/// Эндпоинт для удаления бота из системы.
/// </summary>

using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

/// <summary>
/// Модель запроса для удаления бота
/// </summary>
public class DeleteBotRequest
{
    /// <summary>
    /// Идентификатор бота для удаления
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// Эндпоинт для удаления бота из системы.
/// Выполняет мягкое удаление (soft delete) бота, после которого
/// бот перестает работать в системе.
/// </summary>
public class DeleteBotEndpoint : Endpoint<DeleteBotRequest>
{
    private IBotsManagerService _botsManagerService;
    
    /// <summary>
    /// Инициализирует эндпоинт удаления бота
    /// </summary>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    public DeleteBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Delete("/delete/{Id}");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Удаляем бота из приложения. Бот перестает работать в приложении.";
            s.Description = "SoftDelete сущности бота. Бот перестанет работать.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на удаление бота
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(DeleteBotRequest r, CancellationToken c)
    {
        await _botsManagerService.DeleteBot(r.Id);
        await SendOkAsync();
    }
}

