/// <summary>
/// Эндпоинт для создания и регистрации нового бота в системе.
/// </summary>

using System.ComponentModel;
using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Services.Interfaces;
using MultipleTestBot.Endpoints.Bot;

/// <summary>
/// Модель запроса для создания нового бота
/// </summary>
public class CreateBotRequest
{
    /// <summary>
    /// Токен бота, полученный от BotFather
    /// </summary>
    public string Token { get; set; }
    
    /// <summary>
    /// Опциональный комментарий к боту
    /// </summary>
    [DefaultValue(null)]
    public string? Comment { get; set; }
    
    /// <summary>
    /// Опциональный начальный статус бота
    /// </summary>
    [DefaultValue(null)]
    public BotStatus? Status { get; set; }
}

/// <summary>
/// Эндпоинт для создания нового бота в системе.
/// Обрабатывает запрос на создание бота, регистрирует его в системе
/// и возвращает информацию о созданном боте.
/// </summary>
public class CreateBotEndpoint : Endpoint<CreateBotRequest, BotEntity>
{
    private IBotsManagerService _botsManagerService;
    
    /// <summary>
    /// Инициализирует эндпоинт создания бота
    /// </summary>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    public CreateBotEndpoint(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
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

    /// <summary>
    /// Обрабатывает запрос на создание бота
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(CreateBotRequest r, CancellationToken c)
    {
        var bot = await _botsManagerService.CreateBot(r.Token, r.Comment, r.Status);
        await SendAsync(bot);
    }
}

