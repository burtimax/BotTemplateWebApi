using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Telegram.BotAPI;

namespace MultipleBotFrameworkUpgrade.Models;

public class MyTelegramBotClient : TelegramBotClient, ITelegramBotClient
{
    public delegate ValueTask AsyncEventHandler(
        ITelegramBotClient botClient,
        string method, 
        CancellationToken cancellationToken = default
    );
    
    public event AsyncEventHandler? OnMakingApiRequest;
    
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
    
}