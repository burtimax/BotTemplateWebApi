using System;
using System.Dynamic;
using System.Threading.Tasks;
using BotFramework.Base;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Controllers;

/// <summary>
/// Базовый класс обработчика состояния.
/// </summary>
/// <typeparam name="TResuorces">Тип класса ресурсов. Строки, пути к файлам и т.д.</typeparam>
public abstract class BaseBotState : ControllerBase, IBaseBotHandler
{
    /// <inheritdoc/>
    public BotUser User { get; set; }
    
    /// <inheritdoc/>
    public BotChat Chat { get; set; }
    
    /// <inheritdoc/>
    public BotDbContext BotDbContext { get; set; }
    
    /// <inheritdoc/>
    public string MediaPath { get; set; }
    
    /// <inheritdoc/>
    public ITelegramBotClient BotClient { get; set; }
    
    public BaseBotState(IServiceProvider serviceProvider)
    {
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        BotDbContext = serviceProvider.GetRequiredService<BotDbContext>();
        var botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        MediaPath = botConfig.MediaPath;
    }

    /// <inheritdoc/>
    public abstract Task HandleBotRequest(Update update);
   
}