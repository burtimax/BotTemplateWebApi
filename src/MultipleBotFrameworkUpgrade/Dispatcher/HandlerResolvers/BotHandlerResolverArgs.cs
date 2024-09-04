using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dto;
using MultipleBotFrameworkUpgrade.Enums;

namespace MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;

public class BotHandlerResolverArgs
{
    /// <summary>
    /// Наименование состояния.
    /// </summary>
    public string? StateName { get; set; }
    
    /// <summary>
    /// Команда бота.
    /// Например [/start].
    /// </summary>
    public string? Command { get; set; }
    
    /// <summary>
    /// Тип запроса.
    /// </summary>
    public UpdateType UpdateType { get; set; }
    
    /// <summary>
    /// Пользователь.
    /// </summary>
    public BotUserEntity? User { get; set; }
    
    /// <summary>
    /// Обработчики только для пользователей с определенными разрешениями.
    /// </summary>
    public IEnumerable<ClaimValue>? UserClaims { get; set; }

    /// <summary>
    /// Обработчики только для определенных ботов.
    /// </summary>
    public long BotId { get; set; }
    
    /// <summary>
    /// Роли пользователей.
    /// </summary>
    public string? UserRole { get; set; }

    // /// <summary>
    // /// Версия обработчика
    // /// </summary>
    // public double? Version { get; set; } = 1.0f;
}

