using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dto;
using MultipleBotFrameworkUpgrade.Extensions;
using MultipleBotFrameworkUpgrade.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.Base;

public interface IBaseBotHandler
{
    /// <summary>
    /// DI вервисы.
    /// </summary>
    public IServiceProvider ServiceProvider { get; set; }
    
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
    public IReadOnlyList<ClaimValue>? UserClaims { get; set; }

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
    
    /// <summary>
    /// Отправить приветственное сообщение.
    /// Используется при переходе в состояние.
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task SendIntroduction();
}