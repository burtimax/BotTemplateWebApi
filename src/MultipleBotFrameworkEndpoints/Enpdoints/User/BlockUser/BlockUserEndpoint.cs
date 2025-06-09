/// <summary>
/// Эндпоинт для блокировки и разблокировки пользователей в боте.
/// </summary>

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFrameworkEndpoints.Enpdoints.User;
using MultipleBotFrameworkEndpoints.Extensions;
using MultipleBotFrameworkEndpoints.Models;

/// <summary>
/// Модель запроса для блокировки/разблокировки пользователей
/// </summary>
sealed class BlockUserRequest 
{
    /// <summary>
    /// Список Telegram ID пользователей
    /// </summary>
    public List<long>? TelegramIds { get; set; }
    
    /// <summary>
    /// Список внутренних ID пользователей
    /// </summary>
    public List<long>? Ids { get; set; }
    
    /// <summary>
    /// Список ID ботов для фильтрации
    /// </summary>
    public List<long>? BotIds { get; set; }
    
    /// <summary>
    /// Флаг блокировки: true - заблокировать, false - разблокировать
    /// </summary>
    public bool IsBlocked { get; set; }
}

/// <summary>
/// Эндпоинт для блокировки и разблокировки пользователей в боте.
/// Позволяет блокировать/разблокировать пользователей по их Telegram ID или внутренним ID.
/// </summary>
sealed class BlockUserEndpoint : Endpoint<BlockUserRequest, List<BotChatEntity>>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт блокировки пользователей
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public BlockUserEndpoint(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Post("/block");
        AllowAnonymous();
        Group<UserGroup>();
        Summary(s =>
        {
            s.Summary = "Заблокировать пользователя(лей) в боте.";
            s.Description = $"Если установить {nameof(BlockUserRequest.IsBlocked)} true, тогда блоируем, false - разблокируем.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на блокировку/разблокировку пользователей
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    /// <exception cref="Exception">Выбрасывается при отсутствии идентификаторов пользователей или если пользователи не найдены</exception>
    public override async Task HandleAsync(BlockUserRequest r, CancellationToken c)
    {
        if ((r.Ids is null || r.Ids.Any() == false)
            && (r.TelegramIds is null || r.TelegramIds.Any() == false))
            throw new Exception($"{nameof(r.TelegramIds)} или {nameof(r.Ids)} не должны быть пустые!");
        
        List<BotChatEntity>? chats = await _db.Chats
            .WhereIf(r.TelegramIds is not null && r.TelegramIds.Any(), u => r.TelegramIds.Contains(u.TelegramId))
            .WhereIf(r.Ids is not null && r.Ids.Any(), u => r.Ids.Contains(u.Id))
            .WhereIf(r.BotIds is not null && r.BotIds.Any(), u => r.BotIds!.Contains(u.BotId))
            .ToListAsync();

        if (chats is null || chats.Any() == false)
        {
            throw new Exception("Пользователи не найдены");
            return;
        }

        foreach (var chat in chats)
        {
            chat.IsBlocked = r.IsBlocked;
            _db.Update(chat);
        }
        await _db.SaveChangesAsync();
        
        await SendAsync(chats);
    }
}