using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Models;
using MultipleBotFramework.Options;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils.BotEventHadlers;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Services;

/// <summary>
/// Singleton-фабрика для получения экземпляров Telegram-ботов.
/// </summary>
public class BotFactory : IBotFactory
{
    private readonly IBotsManagerService _botsManagerService;

    /// <summary>
    /// Конструктор фабрики ботов.
    /// </summary>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    public BotFactory(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }
    
    /// <summary>
    /// Получить экземпляр Telegram-бота по ID.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <returns>Экземпляр Telegram-бота или null</returns>
    public async Task<ITelegramBotClient?> GetInstance(long botId)
    {
        return await _botsManagerService.GetBotClientById(botId);
    }
}