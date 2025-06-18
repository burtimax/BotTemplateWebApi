/// <summary>
/// Эндпоинт для получения количества непрочитанных сообщений в чатах.
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
/// Модель запроса для получения новостей чата
/// </summary>
public class GetChatNewsRequest
{
    /// <summary>
    /// Список ID чатов для проверки
    /// </summary>
    public List<long> ChatIds { get; set; }
}

/// <summary>
/// Модель элемента ответа с информацией о непрочитанных сообщениях
/// </summary>
public class GetChatNewsResponseItem
{
    /// <summary>
    /// ID чата
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// ID бота
    /// </summary>
    public long BotId { get; set; }
    
    /// <summary>
    /// Telegram ID чата
    /// </summary>
    public long TelegramId { get; set; }
    
    /// <summary>
    /// Количество непрочитанных сообщений
    /// </summary>
    public long CountNews { get; set; }
}

/// <summary>
/// Эндпоинт для получения количества непрочитанных сообщений в чатах.
/// Возвращает информацию о количестве непрочитанных модератором сообщений для каждого указанного чата.
/// </summary>
public class GetChatNewsEndpoint : Endpoint<GetChatNewsRequest, List<GetChatNewsResponseItem>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения новостей чата
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetChatNewsEndpoint(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Get("/news");
        AllowAnonymous();
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Получение кол-ва непрочитанных модератором сообщений в чатах.";
            s.Description = "Возвращает кол-во непрочитанных сообщений в чатах.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение новостей чата
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetChatNewsRequest r, CancellationToken c)
    {
        List<BotChatEntity> chats = await _db.Chats
            .Where(ch => r.ChatIds.Contains(ch.Id))
            .ToListAsync();

        List<long> chatTelegramIds = chats.Select(ch => ch.TelegramId).ToList();

        var data = await _db.ChatHistory
            .Where(h => h.IsViewed == false)
            .GroupBy(h => new{h.BotId, h.TelegramChatId})
            .Select(h => new
            {
                TelegramId = h.Key.TelegramChatId,
                BotId = h.Key.BotId,
                Count = h.LongCount()
            })
            .ToListAsync();

        List<GetChatNewsResponseItem> result = new();

        foreach (var chat in chats)
        {
            var d = data.FirstOrDefault(i => i.TelegramId == chat.TelegramId && i.BotId == chat.BotId);
            
            result.Add(new()
            {
                Id = chat.Id, 
                TelegramId = chat.TelegramId, 
                BotId = d?.BotId ?? -1, 
                CountNews = d?.Count ?? 0,
            });
        }
        
        await SendAsync(result);
    }
}