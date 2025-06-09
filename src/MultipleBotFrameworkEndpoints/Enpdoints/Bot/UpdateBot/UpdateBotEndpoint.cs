/// <summary>
/// Эндпоинт для обновления данных бота.
/// </summary>

using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

/// <summary>
/// Модель запроса для обновления данных бота
/// </summary>
public class UpdateBotRequest
{
    /// <summary>
    /// Идентификатор бота для обновления
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Новый токен бота (опционально)
    /// </summary>
    public string? Token { get; set; }
    
    /// <summary>
    /// Новый комментарий к боту (опционально)
    /// </summary>
    [DefaultValue(null)]
    public string? Comment { get; set; }
    
    /// <summary>
    /// Новый статус бота (опционально)
    /// </summary>
    [DefaultValue(null)]
    public BotStatus? Status { get; set; }
}

/// <summary>
/// Эндпоинт для обновления данных бота.
/// Позволяет обновить:
/// - Токен бота
/// - Комментарий к боту
/// - Статус бота (включен/выключен/тех. работы)
/// </summary>
public class UpdateBotEndpoint : Endpoint<UpdateBotRequest, BotEntity>
{
    private IBotsManagerService _botsManagerService;
    
    /// <summary>
    /// Инициализирует эндпоинт обновления данных бота
    /// </summary>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    public UpdateBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
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

    /// <summary>
    /// Обрабатывает запрос на обновление данных бота
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(UpdateBotRequest r, CancellationToken c)
    {
        var bot = await _botsManagerService.UpdateBot(r.Id, r.Token, r.Comment, r.Status);
        await SendAsync(bot);
    }
}

