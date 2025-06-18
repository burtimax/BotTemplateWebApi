/// <summary>
/// Эндпоинт для получения списка чатов с поддержкой пагинации, фильтрации и сортировки.
/// </summary>

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using MultipleTestBot.Endpoints.Bot;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.GetChats;

/// <summary>
/// Модель запроса для получения списка чатов
/// </summary>
public class GetChatsRequest : Pagination, IOrdered
{
    /// <summary>
    /// Список ID ботов для фильтрации
    /// </summary>
    public List<long>? BotIds { get; set; }
    
    /// <summary>
    /// Список ID чатов для фильтрации
    /// </summary>
    public List<long>? Ids { get; set; }
    
    /// <summary>
    /// Параметр сортировки
    /// </summary>
    public string? Order { get; set; }
}

/// <summary>
/// Эндпоинт для получения списка чатов.
/// Поддерживает:
/// - Пагинацию результатов
/// - Фильтрацию по ботам и ID чатов
/// - Сортировку результатов
/// </summary>
public class GetChatsEndpoint : Endpoint<GetChatsRequest, PagedList<BotChatEntity>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения списка чатов
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetChatsEndpoint(BotDbContext db)
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
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем сущности чатов.";
            s.Description = "Можем отфильтровать чаты по ботам, можем сортировать по полям.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение списка чатов
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetChatsRequest r, CancellationToken c)
    {
        var query = _db.Chats
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), b => r.BotIds.Contains(b.BotId))
            .WhereIf(r.Ids is not null && r.Ids.Any(), b => r.Ids.Contains(b.Id))
            .Order(r.Order);

        if (string.IsNullOrEmpty(r.Order))
        {
            query.OrderByDescending(c => c.UpdatedAt);
        }

        var result = await PagedList<BotChatEntity>.ToPagedListAsync(query, r);
        await SendAsync(result);
    }
}