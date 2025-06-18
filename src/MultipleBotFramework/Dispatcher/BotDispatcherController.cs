using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Exceptions;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services;
using MultipleBotFramework.Utils;
using MultipleBotFramework.Utils.ExceptionHandler;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Dispatcher;

/// <summary>
/// Контроллер для обработки запросов от Telegram-бота.
/// </summary>
[Controller]
public class BotDispatcherController : BaseBotController
{
    private readonly BotUpdateDispatcher _dispatcher;

    /// <summary>
    /// Конструктор контроллера диспетчера бота.
    /// </summary>
    /// <param name="dispatcher">Диспетчер обновлений бота</param>
    public BotDispatcherController(BotUpdateDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    /// <summary>
    /// Обрабатывает запрос от бота.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="update">Обновление Telegram</param>
    /// <returns>Результат обработки запроса</returns>
    public override async Task<IActionResult> HandleBotRequest(long botId, Update update)
    {
        await _dispatcher.HandleBotRequest(botId, update);
        return Ok();
    }
}