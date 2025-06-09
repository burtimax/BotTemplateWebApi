/// <summary>
/// Эндпоинт для перезапуска и обновления информации о боте.
/// </summary>

using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

/// <summary>
/// Модель запроса для обновления бота
/// </summary>
public class RenewBotRequest
{
    /// <summary>
    /// Идентификатор бота для обновления
    /// </summary>
    public long Id { get; set; }
}

/// <summary>
/// Эндпоинт для перезапуска и обновления информации о боте.
/// Выполняет:
/// - Перезагрузку бота
/// - Обновление данных вебхука
/// - Добавление бота в кеш менеджера ботов
/// </summary>
public class RenewBotEndpoint : Endpoint<RenewBotRequest, BotEntity>
{
    private IBotsManagerService _botsManagerService;

    /// <summary>
    /// Инициализирует эндпоинт обновления бота
    /// </summary>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    public RenewBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
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

    /// <summary>
    /// Обрабатывает запрос на обновление бота
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(RenewBotRequest r, CancellationToken c)
    {
        var bot = await _botsManagerService.RenewBotInfo(r.Id);
        await SendAsync(bot);
    }
}

