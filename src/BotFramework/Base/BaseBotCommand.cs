using System;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Base;

/// <summary>
/// Базовый класс обработчика команды.
/// </summary>
public abstract class BaseBotCommand : ControllerBase, IBaseBotHandler
{
    /// <inheritdoc/>
    public BotUser User { get; set; }
    
    /// <inheritdoc/>
    public BotChat Chat { get; set; }
    
    /// <inheritdoc/>
    public BotDbContext BotDbContext { get; set; }
    
    /// <inheritdoc/>
    public string MediaDirectory { get; set; }
    
    /// <inheritdoc/>
    public ITelegramBotClient BotClient { get; set; }

    public BaseBotCommand(IServiceProvider serviceProvider)
    {
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        BotDbContext = serviceProvider.GetRequiredService<BotDbContext>();
        var botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        MediaDirectory = botConfig.MediaDirectory;
    }

    /// <inheritdoc/>
    public abstract Task HandleBotRequest(Update update);
    
}