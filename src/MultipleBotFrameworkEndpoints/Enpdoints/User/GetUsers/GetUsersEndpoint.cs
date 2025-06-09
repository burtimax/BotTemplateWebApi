/// <summary>
/// Эндпоинт для получения списка пользователей с возможностью фильтрации и пагинации.
/// </summary>

using FastEndpoints;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Enpdoints.User;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

/// <summary>
/// Модель запроса для получения списка пользователей
/// </summary>
sealed class GetUsersRequest : Pagination, IOrdered
{
    /// <summary>
    /// Список ID пользователей для фильтрации
    /// </summary>
    public List<long>? Ids { get; set; }
    
    /// <summary>
    /// Список ID ботов для фильтрации
    /// </summary>
    public List<long>? BotIds { get; set; }
    
    /// <summary>
    /// Параметр сортировки
    /// </summary>
    public string? Order { get; set; }
}

/// <summary>
/// Эндпоинт для получения списка пользователей.
/// Поддерживает фильтрацию по ботам и ID, пагинацию и сортировку результатов.
/// </summary>
sealed class GetUsersEndpoint : Endpoint<GetUsersRequest, PagedList<BotUserEntity>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения списка пользователей
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetUsersEndpoint(BotDbContext db)
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
        Group<UserGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем список пользователей.";
            s.Description = "Можем отфильтровать сущности пользователей по ботам, по ИД, пагинировать и сотрировать.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение списка пользователей
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetUsersRequest r, CancellationToken c)
    {
        var query = _db.Users
            .WhereIf(r.Ids is not null && r.Ids.Any(), b => r.Ids!.Contains(b.Id))
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), b => r.BotIds!.Contains(b.BotId))
            .Order(r.Order);

        var users = await PagedList<BotUserEntity>.ToPagedListAsync(query, r);
        await SendAsync(users);
    }
}