using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Models;
using MultipleBotFramework.Options;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;
using MultipleBotFramework.Utils.BotEventHadlers;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Services;

public class BotsManagerService : IBotsManagerService
{
    private static Dictionary<long,MyTelegramBotClient> botCache = new();
    private static bool cacheInitialized = false;
    private BotDbContext _db;
    private BotOptions _botOptions;
    
    public BotsManagerService(BotDbContext db, IOptions<BotOptions> botOptions,
        IOptions<BotConfiguration> config)
    {
        _db = db;
        _botOptions = botOptions.Value;
        var c = config.Value;
        
        if (string.IsNullOrEmpty(c.Webhook))
            throw new Exception("Required webhook address in configuratin");
        BotWebhook.BaseAddress = c.Webhook;
        MyTelegramBotClient.BotDbConnection = c.DbConnection;
    }

    public async Task<MyTelegramBotClient?> GetBotClientById(long botId)
    {
        if (botId == default) return null;
        return await GetFromCacheSafe(botId);
    }

    public async Task<BotEntity?> GetBotById(long botId)
    {
        return await _db.Bots.FirstOrDefaultAsync(b => b.Id == botId);
    }

    public async Task<BotEntity> CreateBot(string token, string? comment = null, BotStatus? status = null)
    {
        token = token.Trim(' ');

        var botClient = new MyTelegramBotClient(token);
        try
        {
            await botClient.GetMeAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Токен не валидный.");
        }
        
        var bot = await _db.Bots.FirstOrDefaultAsync(b => b.Token == token);

        if (bot == null) bot = new();

        if (comment is not null) bot.Comment = comment;
        if (status is not null) bot.Status = status.Value;
        bot.Token = token;

        if (bot.Id == default) _db.Bots.Add(bot);
        else _db.Bots.Update(bot);

        await _db.SaveChangesAsync();
        await UpdateInCache(bot);
        return await RenewBotInfo(bot.Id);
        return bot;
    }
    
    public async Task<BotEntity> UpdateBot(long id, string? token, string? comment = null, BotStatus? status = null)
    {
        BotEntity bot = (await _db.Bots.FirstOrDefaultAsync(b => b.Id == id)) ?? throw new Exception($"Не найден бот [{id}]");

        if (string.IsNullOrEmpty(token) == false)
        {
            var existed = await _db.Bots.FirstOrDefaultAsync(b => b.Token == token);
            if (existed is not null && existed.Id != bot.Id) throw new Exception("Бот с таким токеном уже существует");
            
            token = token?.Trim(' ');
            var botClient = new MyTelegramBotClient(token);
            try
            {
                await botClient.GetMeAsync();
            }
            catch (Exception e)
            {
                throw new Exception("Токен не валидный.");
            }

            bot.Token = token;
        }
        
        if (string.IsNullOrEmpty(comment) == false) bot.Comment = comment;
        if (status is not null) bot.Status = status.Value;

        _db.Bots.Update(bot);
        await _db.SaveChangesAsync();
        await UpdateInCache(bot);
        return await RenewBotInfo(bot.Id);
        return bot;
    }

    public async Task DeleteBot(long id)
    {
        var bot = await GetBotById(id);
        await UpdateInCache(id);
        if (bot is not null)
        {
            _db.Bots.Remove(bot);
            await _db.SaveChangesAsync();
        }
    }
    
    public async Task<BotEntity> RenewBotInfo(long botId)
    {
        BotEntity bot = (await GetBotById(botId)) ?? throw new Exception($"Не найден бот [{botId}]");

        var botClient = await GetFromCacheSafe(botId);
        
        var desc = await botClient.GetMyDescriptionAsync();
        bot.Description = desc.Description;
        
        var shortDesc = await botClient.GetMyShortDescriptionAsync();
        bot.ShortDescription = shortDesc.ShortDescription;

        var botInfo = await botClient.GetMeAsync();
        bot.Username = $"@{botInfo.Username}";

        var name = await botClient.GetMyNameAsync();
        bot.Name = name.Name;

        _db.Bots.Update(bot);
        await _db.SaveChangesAsync();
        return bot;
    }

    private async Task<MyTelegramBotClient> GetFromCacheSafe(long botId)
    {
        await InitializeCacheIfNeed();
        
        if (botCache.ContainsKey(botId)) return botCache[botId];

        await UpdateInCache(botId);
        return botCache[botId];
    }

    private async Task InitializeCacheIfNeed()
    {
        if(cacheInitialized) return;
        List<BotEntity> bots = await _db.Bots.ToListAsync();
        
        if(bots is null || bots.Any() == false) return;
        
        foreach (var bot in bots)
        {
            await UpdateInCache(bot);
        }
        cacheInitialized = true;
    }

    private async Task UpdateInCache(BotEntity bot)
    {
        if (botCache.ContainsKey(bot.Id))
        {
            var item = botCache[bot.Id];
            item.OnMakingApiRequest -= Throttling.Handler;
            item = null;
            botCache.Remove(bot.Id);
        }

        try
        {
            if (bot is null) throw new Exception($"Not found bot in db [{bot.Id}]");
        
            MyTelegramBotClient botClient = new(bot.Id, bot.Token);
        
            await botClient.SetWebhookAsync(BotWebhook.GetWebhookForBot(bot.Id));

            if (bot.Webhook != BotWebhook.GetWebhookForBot(bot.Id))
            {
                bot.Webhook = BotWebhook.GetWebhookForBot(bot.Id);
                _db.Bots.Update(bot);
                await _db.SaveChangesAsync();
            }
                
            if (_botOptions != null && _botOptions.BoundRequestsInSecond != null)
            {
                Throttling.BoundRequestInSecond = _botOptions.BoundRequestsInSecond.Value;
                botClient.OnMakingApiRequest += Throttling.Handler; 
            }
                
            botCache.Add(bot.Id, botClient);
        }
        catch (Exception e)
        {
        }
    }
    private async Task UpdateInCache(long botId)
    {
        var bot = await GetBotById(botId);
        if (bot is null)
        {
            if (botCache.ContainsKey(botId)) botCache.Remove(botId);
            return;
        }
        await UpdateInCache(bot);
        
    }
}