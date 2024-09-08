using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Models;

public class MyTelegramBotClient : TelegramBotClient, ITelegramBotClient
{
    public static string? BotDbConnection;
    private static BotDbContext? _db;
    private const int _dbMaxGetCount = 2000;
    private static int _dbGetCount = 0;
    
    public delegate ValueTask AsyncEventHandler(
        ITelegramBotClient botClient,
        string method, 
        CancellationToken cancellationToken = default
    );
    
    public event AsyncEventHandler? OnMakingApiRequest;

    /// <summary>
    /// ИД сущности бота.
    /// </summary>
    public long? BotId { get; set; }
    
    public MyTelegramBotClient(long botId, string botToken, HttpClient? httpClient = null) : this(botToken, httpClient)
    {
        BotId = botId;
    }
    
    public MyTelegramBotClient(string botToken, HttpClient? httpClient = null) : base(botToken, httpClient)
    {
    }

    public MyTelegramBotClient(TelegramBotClientOptions options, HttpClient? httpClient = null) : base(options, httpClient)
    {
    }

    TResult ITelegramBotClient.CallMethod<TResult>(string method, object? args = null)
    {
        OnMakingApiRequest?.Invoke(this, method);
        var result = base.CallMethod<TResult>(method, args);
        SaveResultIfNeed(method, args, result).ConfigureAwait(false).GetAwaiter();
        return result;
    }
    
    async Task<TResult> ITelegramBotClient.CallMethodAsync<TResult>(string method, object? args = null, CancellationToken cancellationToken = default)
    {
        OnMakingApiRequest?.Invoke(this, method, cancellationToken);
        var result = await base.CallMethodAsync<TResult>(method, args, cancellationToken);
        await SaveResultIfNeed(method, args, result);
        return result;
    }

    BotResponse<TReturn> ITelegramBotClient.CallMethodDirect<TReturn>(string method, object? args = null)
    {
        OnMakingApiRequest?.Invoke(this, method);
        var result = base.CallMethodDirect<TReturn>(method, args);
        SaveResultIfNeed(method, args, result).ConfigureAwait(false).GetAwaiter();
        return result;
    }

    async Task<BotResponse<TReturn>> ITelegramBotClient.CallMethodDirectAsync<TReturn>(string method, object? args = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        OnMakingApiRequest?.Invoke(this, method);
        var result = await base.CallMethodDirectAsync<TReturn>(method, args, cancellationToken);
        await SaveResultIfNeed(method, args, result);
        return result;
    }

    private async Task SaveResultIfNeed<TResult>(string method, object? args, TResult result)
    {
        if(args is null) return;
        if(BotId is null) return;
        if(TryGetChatIdFromArgs(args, out long chatId) == false) return;
        
        BotDbContext? db = GetDb();
        if(db is null) return;
        
        BotChatHistoryService chatHistoryService = new(db);
        await chatHistoryService.SaveInChatHistoryIfNeed(BotId.Value, chatId, true, result);
    }
    
    private BotDbContext? GetDb()
    {
        if (string.IsNullOrEmpty(BotDbConnection)) return null;

        if (_dbGetCount > _dbMaxGetCount)
        {
            _db.Dispose();
            _db = null;
        }
        
        if (_db is null)
        {
            var ob = new DbContextOptionsBuilder<BotDbContext>();
            ob.UseNpgsql(BotDbConnection);
            _db = new BotDbContext(ob.Options);
        }

        _dbGetCount++;
        return _db;
    }
    
    
    private bool TryGetChatIdFromArgs(object args, out long chatId)
    {
        chatId = 0;
        if (typeof(IEnumerable).IsAssignableFrom(args.GetType()))
        {
            if (args is IDictionary<string, object> items)
            {
                foreach (var item in items)
                {
                    if (item.Key == PropertyNames.ChatId)
                    {
                        return long.TryParse(item.Value.ToString(), out chatId);
                    }
                }
            }
        }
        else
        {
            var properties = args.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(args);
                if (property.Name == PropertyNames.ChatId || property.Name == "ChatId")
                {
                    return long.TryParse(value.ToString(), out chatId);
                }
            }
        }

        return false;
    }
    
}