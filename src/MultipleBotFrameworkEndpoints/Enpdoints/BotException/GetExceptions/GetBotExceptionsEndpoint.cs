/// <summary>
/// Эндпоинт для получения списка исключений с поддержкой пагинации, фильтрации и сортировки.
/// </summary>

using FastEndpoints;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Enpdoints.Chat;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotException.GetExceptions;

/// <summary>
/// Модель запроса для получения списка исключений
/// </summary>
public class GetBotExceptionsRequest : Pagination, IOrdered
{
    /// <summary>
    /// Список ID ботов для фильтрации
    /// </summary>
    public List<long>? BotIds { get; set; }
    
    /// <summary>
    /// Список ID исключений для фильтрации
    /// </summary>
    public List<long>? Ids { get; set; }
    
    /// <summary>
    /// Параметр сортировки
    /// </summary>
    public string? Order { get; set; }
}

/// <summary>
/// Эндпоинт для получения списка исключений.
/// Поддерживает:
/// - Пагинацию результатов
/// - Фильтрацию по ботам и ID исключений
/// - Сортировку результатов
/// </summary>
public class GetBotExceptionsEndpoint : Endpoint<GetBotExceptionsRequest, PagedList<BotExceptionEntity>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения исключений
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetBotExceptionsEndpoint(BotDbContext db)
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
        Group<ExceptionGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем список ошибок в приложении.";
            s.Description = "Необходимо чтобы отслеживать работу приложения, делать своевременные доработки.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение списка исключений
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetBotExceptionsRequest r, CancellationToken c)
    {
        var query = _db.Exceptions
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), e => e.BotId != null && r.BotIds.Contains(e.BotId.Value))
            .WhereIf(r.Ids is not null && r.Ids.Any(), e => r.Ids.Contains(e.Id))
            .Order(r.Order);

        var result = await PagedList<BotExceptionEntity>.ToPagedListAsync(query, r);
        await SendAsync(result);
    }
}