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
/// Singleton фабрика ботов.
/// </summary>
public class BotFactory : IBotFactory
{
    private readonly IBotsManagerService _botsManagerService;

    public BotFactory(IBotsManagerService botsManagerService)
    {
        _botsManagerService = botsManagerService;
    }
    
    public async Task<ITelegramBotClient?> GetInstance(long botId)
    {
        return await _botsManagerService.GetBotClientById(botId);
    }
}