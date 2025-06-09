/// <summary>
/// Эндпоинт для временного отключения чата.
/// </summary>

using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat.DisableChat;

/// <summary>
/// Модель запроса для отключения чата
/// </summary>
public class DisableChatRequest
{
    /// <summary>
    /// ID чата для отключения
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Время отключения в секундах. Если 0 - чат активируется
    /// </summary>
    public long DisableForSeconds { get; set; }
}

/// <summary>
/// Эндпоинт для временного отключения чата.
/// Позволяет временно отключить чат, чтобы бот не отвечал на сообщения.
/// В это время можно общаться с пользователем через админку.
/// </summary>
public class DisableChatEndpoint : Endpoint<DisableChatRequest, BotChatEntity>
{
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт отключения чата
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    public DisableChatEndpoint(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Post("/disable");
        AllowAnonymous();
        Group<ChatGroup>();
        Summary(s =>
        {
            s.Summary = "Делаем чат неактивным.";
            s.Description = "Указываем время (в секундах), которое чат будет неактивным. " +
                            "Когда отправляем 0 сек - чат становится активным. В момент неактивности бот не отвечает (можно общаться через админку).";
            
        });
    }

    /// <summary>
    /// Обрабатывает запрос на отключение чата
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    /// <exception cref="Exception">Выбрасывается, если чат не найден</exception>
    public override async Task HandleAsync(DisableChatRequest r, CancellationToken c)
    {
        var chat = await _db.Chats
            .FirstOrDefaultAsync(c => c.Id == r.Id);

        if (chat is null) throw new Exception($"Не найден чат [Id = {r.Id}]");

        if (r.DisableForSeconds <= 0) chat.DisabledUntil = null;
        
        chat.DisabledUntil = DateTimeOffset.Now.AddSeconds(r.DisableForSeconds);

        await _db.SaveChangesAsync();
        
        await SendAsync(chat);
    }
}