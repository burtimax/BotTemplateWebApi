﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Models;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Utils.BotEventHadlers;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.Services;

/// <summary>
/// Singleton фабрика ботов.
/// </summary>
public class BotFactory : IBotFactory
{
    private static List<(long botId,ITelegramBotClient botClient)> _bots = new();

    public BotFactory(BotDbContext db, BotConfiguration config, BotOptions botOptions)
    {
        List<BotEntity> bots = new();
        bots = db.Bots.ToList();
        
        foreach (var bot in bots)
        {
            try
            {
                MyTelegramBotClient botClient = new(bot.Token);
                string webhook = config.Webhook.TrimEnd('/') + '/' + bot.Id;
                botClient.SetWebhookAsync(webhook).Wait();
                
                if (botOptions != null && botOptions.BoundRequestsInSecond != null)
                {
                    OnMakingApiRequest.BoundRequestInSecond = botOptions.BoundRequestsInSecond.Value;
                    botClient.OnMakingApiRequest += OnMakingApiRequest.Handler; 
                }
                
                _bots.Add((bot.Id, botClient));
            }
            catch (Exception e)
            {
            }
        }
        
    }
    
    public ITelegramBotClient? GetInstance(long botId)
    {
        return _bots.FirstOrDefault(b => b.botId == botId).botClient;
    }
}