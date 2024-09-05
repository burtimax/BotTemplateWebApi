using System;
using System.Collections.Generic;
using System.Linq;
using MultipleBotFramework.Enums;

namespace MultipleBotFramework.Dispatcher.HandlerResolvers;

[AttributeUsage(AttributeTargets.Class)]
public class BotHandlerAttribute : Attribute
{
    /// <summary>
    /// Команда бота, требуемая для обработчика.
    /// Например [/start]
    /// </summary>
    public string? Command { get; set; }
    
    /// <summary>
    /// Версия обработчика.
    /// </summary>
    public double Version { get; set; }
    
    /// <summary>
    /// Роль пользователя, требуемая для обработчика.
    /// Пользователи каких ролей будут попадать в обработчик.
    /// </summary>
    public IEnumerable<string>? UserRoles { get; set; }
    
    /// <summary>
    /// Запросы какого бота будет получать обработчик.
    /// </summary>
    public IEnumerable<long>? BotIds { get; set; }
    
    /// <summary>
    /// Какого типа должно быть обновление.
    /// </summary>
    public IEnumerable<UpdateType>? UpdateTypes { get; set; }
    
    /// <summary>
    /// Наименование состояния для обработчика.
    /// </summary>
    public string? StateName { get; set; }

    /// <summary>
    /// Пользователи с одним из этих разрешений будут попадать в обработчик.
    /// </summary>
    public IEnumerable<string>? UserClaims { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="stateName">Наименование состояния для обработчика.</param>
    /// <param name="updateTypes">Какого типа должно быть обновление.</param>
    /// <param name="command">Команда бота, требуемая для обработчика. Например [/start]</param>
    /// <param name="userRoles">Пользователи каких ролей будут попадать в обработчик.</param>
    /// <param name="requiredUserClaims">Пользователи с одним из этих разрешений будут попадать в обработчик.</param>
    /// <param name="version">Версия обработчика.</param>
    /// <param name="botIds">Запросы какого бота будет получать обработчик.</param>
    public BotHandlerAttribute(
        string? stateName = null,
        UpdateType[]? updateTypes = null,
        string? command = null,
        string[]? userRoles = null,
        string[]? requiredUserClaims = null,
        double version = 1.0f,
        long[] botIds = null)
    {
        this.StateName = stateName;
        this.UpdateTypes = updateTypes;
        this.Command = command;
        this.UserRoles = userRoles?.ToList();
        this.UserClaims = requiredUserClaims?.ToList();
        this.BotIds = botIds?.ToList();
        this.Version = version;
    }
}