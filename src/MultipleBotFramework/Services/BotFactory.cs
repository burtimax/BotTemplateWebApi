using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Options;
using MultipleBotFramework.Utils.BotEventHadlers;
using Telegram.Bot;

namespace MultipleBotFramework.Services;

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