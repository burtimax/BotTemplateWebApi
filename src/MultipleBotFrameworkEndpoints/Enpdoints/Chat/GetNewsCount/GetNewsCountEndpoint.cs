/// <summary>
/// Эндпоинт для получения общего количества чатов с непрочитанными сообщениями.
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
/// Модель запроса для получения количества новостей
/// </summary>
public class GetNewsCountRequest
{
    /// <summary>
    /// Список ID ботов для фильтрации
    /// </summary>
    public List<long>? BotIds { get; set; }
}

/// <summary>
/// Модель ответа с количеством чатов с новостями
/// </summary>
public class GetNewsCountResponse
{
    /// <summary>
    /// Количество чатов с непрочитанными сообщениями
    /// </summary>
    public long CountChatNews { get; set; }
}

/// <summary>
/// Эндпоинт для получения общего количества чатов с непрочитанными сообщениями.
/// Возвращает количество уникальных чатов, в которых есть непрочитанные сообщения.
/// </summary>
public class GetNewsCountEndpoint : Endpoint<GetNewsCountRequest, GetNewsCountResponse>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения количества новостей
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetNewsCountEndpoint(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Get("/news-count");
        AllowAnonymous();
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Кол-во непрочитанных чатов.";
            s.Description = "Показывает кол-во чатов, в которых есть непрочитанные сообщения.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение количества новостей
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetNewsCountRequest r, CancellationToken c)
    {
        var count = await _db.ChatHistory
            .Where(h => h.IsViewed == false)
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), h => r.BotIds!.Contains(h.BotId))
            .GroupBy(h => new {h.BotId, h.TelegramChatId})
            .Select(h => new
            {
                TelegramId = h.Key.TelegramChatId,
                BotId = h.Key.BotId,
                Count = h.LongCount()
            })
            .LongCountAsync();
        
        await SendAsync(new ()
        {
            CountChatNews = count,
        });
    }
}