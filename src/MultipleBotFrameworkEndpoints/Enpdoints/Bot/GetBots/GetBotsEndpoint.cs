/// <summary>
/// Эндпоинт для получения списка ботов с поддержкой пагинации, фильтрации и сортировки.
/// </summary>

using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using MultipleTestBot.Endpoints.Bot;

/// <summary>
/// Модель запроса для получения списка ботов
/// </summary>
public class GetBotsRequest : Pagination, IOrdered
{
    /// <summary>
    /// Список идентификаторов ботов для фильтрации
    /// </summary>
    public List<long>? Ids { get; set; }
    
    /// <summary>
    /// Параметр сортировки
    /// </summary>
    public string? Order { get; set; }
}

/// <summary>
/// Эндпоинт для получения списка ботов.
/// Поддерживает:
/// - Пагинацию результатов
/// - Фильтрацию по идентификаторам
/// - Сортировку результатов
/// </summary>
public class GetBotsEndpoint : Endpoint<GetBotsRequest, PagedList<BotEntity>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения списка ботов
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetBotsEndpoint(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Get("/get");
        AllowAnonymous();
        Group<BotGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем список ботов.";
            s.Description = "Сущности ботов. Можем фильтровать, пагинировать, сортировать.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение списка ботов
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetBotsRequest r, CancellationToken c)
    {
        var query = _db.Bots
            .WhereIf(r.Ids is not null && r.Ids.Any(), b => r.Ids.Contains(b.Id))
            .Order(r.Order);

        var bots = await PagedList<BotEntity>.ToPagedListAsync(query, r);
        await SendAsync(bots);
    }
}

