/// <summary>
/// Эндпоинт для получения истории сообщений чата с поддержкой пагинации, фильтрации и сортировки.
/// </summary>

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.GetChatHistory;

/// <summary>
/// Модель запроса для получения истории сообщений чата
/// </summary>
public class GetChatHistoryRequest : Pagination, IOrdered
{
    /// <summary>
    /// Список ID ботов для фильтрации
    /// </summary>
    public List<long>? BotIds { get; set; }
    
    /// <summary>
    /// Список Telegram ID чатов для фильтрации
    /// </summary>
    public List<long>? ChatTelegramIds { get; set; }
    
    /// <summary>
    /// Параметр сортировки
    /// </summary>
    public string? Order { get; set; }
}

/// <summary>
/// Эндпоинт для получения истории сообщений чата.
/// Поддерживает:
/// - Пагинацию результатов
/// - Фильтрацию по ботам и чатам
/// - Сортировку результатов
/// - Отметку сообщений как прочитанных
/// - Подгрузку ответных сообщений
/// </summary>
public class GetChatHistoryEndpoint : Endpoint<GetChatHistoryRequest, PagedList<BotChatHistoryEntity>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт получения истории чата
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public GetChatHistoryEndpoint(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Get("/history/get");
        AllowAnonymous();
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Получаем историю сообщений.";
            s.Description = "Можем отфильтровать данные и смотреть историю сообщений чата (чатов).";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение истории сообщений
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetChatHistoryRequest r, CancellationToken c)
    {
        var query = _db.ChatHistory
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), b => r.BotIds.Contains(b.BotId))
            .WhereIf(r.ChatTelegramIds is not null && r.ChatTelegramIds.Any(), b => r.ChatTelegramIds.Contains(b.TelegramChatId))
            .Order(r.Order);

        if (string.IsNullOrEmpty(r.Order))
        {
            query.OrderByDescending(i => i.CreatedAt);
        }
        
        var result = await PagedList<BotChatHistoryEntity>.ToPagedListAsync(query, r);

        // Если получаем историю только по одному чату, тогда обновляем данные о просмотре сообщений чата.
        if (r.BotIds is not null && r.BotIds.Count == 1
            && r.ChatTelegramIds is not null && r.ChatTelegramIds.Count == 1 
            && result is not null && result.Data is not null && result.Data.Any())
        {
            await UpdateIsViewed(r.BotIds.First(), r.ChatTelegramIds.First(),result.Data);
        }

        // Подгружаем ответные сообщения.
        if (result.Data is not null && result.Data.Any())
        {
            List<(long chatId, long messageId)> required = result.Data.Where(r => r.ReplyToMessageId is not null)
                .Select(r => (r.TelegramChatId, r.ReplyToMessageId!.Value)).ToList();
            if (required is not null && required.Any())
            {
                var repliedMessages = await GetRepliedMessages(required);
                for (int i = 0; i < result.Data.Count; i++)
                {
                    var item = result.Data[i];
                    var replied = repliedMessages.FirstOrDefault(r => r.MessageId == item.ReplyToMessageId && r.TelegramChatId == item.TelegramChatId);
                    if (replied is not null)
                    {
                        item.ReplyToMessage = replied;
                    }
                }
            }
            
        }
        
        await SendAsync(result);
    }

    /// <summary>
    /// Получает список ответных сообщений
    /// </summary>
    /// <param name="required">Список пар (ID чата, ID сообщения)</param>
    /// <returns>Список ответных сообщений</returns>
    private async Task<List<BotChatHistoryEntity>?> GetRepliedMessages(List<(long chatId, long messageId)> required)
    {
        if (required is null || required.Any() == false) return null;
        
        IQueryable<BotChatHistoryEntity> q = _db.ChatHistory;   

        for(var i = 0; i < required.Count; i++){
            var chatId = required[i].chatId;
            var messageId = required[i].messageId;
            q = q.Where(e => e.TelegramChatId == chatId && e.MessageId == messageId); 
        }

        return await q.ToListAsync();
    }

    /// <summary>
    /// Обновляет статус просмотра сообщений в чате
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="telegramChatId">Telegram ID чата</param>
    /// <param name="items">Список сообщений</param>
    private async Task UpdateIsViewed(long botId, long telegramChatId, List<BotChatHistoryEntity> items)
    {
        var last = items.OrderByDescending(i => i.CreatedAt).First();
        await _db.ChatHistory.Where(h => 
                h.BotId == botId 
                && h.TelegramChatId == telegramChatId
                && h.CreatedAt <= last.CreatedAt)
            .ExecuteUpdateAsync(s => s.SetProperty(h => h.IsViewed, true));
        await _db.SaveChangesAsync();
    }
}