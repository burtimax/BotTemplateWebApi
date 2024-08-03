using System.Collections.Generic;
using System.Threading.Tasks;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Extensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MultipleBotFramework.Base;

public interface IBaseBotHandler
{
    /// <summary>
    /// Идентификатор бота.
    /// </summary>
    public long BotId { get; set; }
    
    /// <summary>
    /// Сущность пользователя (отслеживаемая EF core).
    /// </summary>
    /// <remarks>
    /// Может быть NULL в определенных типах запроса.
    /// <seealso cref="UpdateExtensions.GetUser"/>
    /// </remarks>
    public BotUserEntity? User { get; set; }

    /// <summary>
    /// Сущность чата (отслеживаемая EF core).
    /// </summary>
    /// <remarks>
    /// Может быть NULL в определенных типах запроса.
    /// <seealso cref="UpdateExtensions.GetUser"/>
    /// </remarks>
    public BotChatEntity? Chat { get; set; }

    /// <summary>
    /// Запрос.
    /// </summary>
    public Update Update { get; set; }
    
    /// <summary>
    /// Контекст базы данных бота.
    /// </summary>
    public BotDbContext BotDbContext { get; set; }
    
    /// <summary>
    /// Путь к директории медиафайлов бота.
    /// </summary>
    public string MediaDirectory { get; set; }
    
    /// <summary>
    /// Клиент телеграм АПИ.
    /// </summary>
    public ITelegramBotClient BotClient { get; set; }
    
    /// <summary>
    /// Разрешения пользователя.
    /// </summary>
    public IReadOnlyList<ClaimValue> UserClaims { get; set; }

    /// <summary>
    /// Является ли текущий пользователь владельцем бота?
    /// </summary>
    public bool IsOwner { get; set; }
    
    /// <summary>
    /// Метод-обработчик запроса.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    Task HandleBotRequest(Update update);
}