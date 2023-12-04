using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Base;

public interface IBaseBotHandler
{
    /// <summary>
    /// Сущность пользователя (отслеживаемая EF core).
    /// </summary>
    public BotUser User { get; set; }

    /// <summary>
    /// Сущность чата (отслеживаемая EF core).
    /// </summary>
    public BotChat Chat { get; set; }

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
    /// Метод-обработчик запроса.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    Task HandleBotRequest(Update update);
}