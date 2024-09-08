using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using Telegram.BotAPI;

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
        return base.CallMethod<TResult>(method, args);
    }
    
    Task<TResult> ITelegramBotClient.CallMethodAsync<TResult>(string method, object? args = null, CancellationToken cancellationToken = default)
    {
        OnMakingApiRequest?.Invoke(this, method, cancellationToken);
        return base.CallMethodAsync<TResult>(method, args, cancellationToken);
    }

    BotResponse<TReturn> ITelegramBotClient.CallMethodDirect<TReturn>(string method, object? args = null)
    {
        OnMakingApiRequest?.Invoke(this, method);
        return base.CallMethodDirect<TReturn>(method, args);
    }

    Task<BotResponse<TReturn>> ITelegramBotClient.CallMethodDirectAsync<TReturn>(string method, object? args = null,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        OnMakingApiRequest?.Invoke(this, method);
        return base.CallMethodDirectAsync<TReturn>(method, args, cancellationToken);
    }

    private async Task SaveResult<TResult>(string method, TResult result)
    {
        BotDbContext? db = GetDb();
        if(db is null) return;
        
        // ISaveUpdateService saveUpdateService = new SaveUpdateService(db);
        // saveUpdateService.
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
}