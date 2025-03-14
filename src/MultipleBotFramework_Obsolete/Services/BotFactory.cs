﻿using System;
using System.Collections.Generic;
using System.Linq;
using MultipleBotFramework_Obsolete.Db;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Options;
using MultipleBotFramework_Obsolete.Utils.BotEventHadlers;
using Telegram.Bot;

namespace MultipleBotFramework_Obsolete.Services;

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
                TelegramBotClient botClient = new(bot.Token);
                string webhook = config.Webhook.TrimEnd('/') + '/' + bot.Id;
                botClient.SetWebhookAsync(webhook, allowedUpdates: BotConfiguration.AllAllowedUpdates).Wait();
                
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