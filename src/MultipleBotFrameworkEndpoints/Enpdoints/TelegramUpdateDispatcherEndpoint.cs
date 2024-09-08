using FastEndpoints;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dispatcher;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

sealed class TelegramUpdateDispatcherRequest
{
    public long BotId { get; set; }
    
    [FromBody]
    public Update Update { get; set; }
}

sealed class TelegramUpdateDispatcherResponse
{

}

sealed class TelegramUpdateDispatcherEndpoint : Endpoint<TelegramUpdateDispatcherRequest>
{
    private BotUpdateDispatcher _updateDispatcher;
    private readonly IBotsManagerService _botsManager;
    private readonly IBotFactory _botFactory;

    public TelegramUpdateDispatcherEndpoint(BotUpdateDispatcher updateDispatcher, 
        IBotsManagerService botsManager, IBotFactory botFactory)
    {
        _updateDispatcher = updateDispatcher;
        _botsManager = botsManager;
        _botFactory = botFactory;
    }

    public override void Configure()
    {
        Post(string.Format(BotWebhook.WebhookRouteFormat, $"{{{nameof(TelegramUpdateDispatcherRequest.BotId)}}}"));
        AllowAnonymous();
    }

    public override async Task HandleAsync(TelegramUpdateDispatcherRequest r, CancellationToken c)
    {
        BotEntity? bot = await _botsManager.GetBotById(r.BotId);
        var telegramChat = r.Update.GetChat();
        if (bot is not null && bot.Status != BotStatus.On)
        {
            ITelegramBotClient? botClient = await _botFactory.GetInstance(r.BotId);
            if (bot.Status == BotStatus.TechWork && botClient is not null && telegramChat is not null)
            {
                await botClient.SendMessageAsync(telegramChat.Id, "Проводим тeхнические работы. Скоро бот заработает.");
                await SendOkAsync();
                return;
            }
            
            throw new Exception($"Bot [{r.BotId}] is off");
        }
        
        await _updateDispatcher.HandleBotRequest(r.BotId, r.Update);
        await SendOkAsync();
    }
}