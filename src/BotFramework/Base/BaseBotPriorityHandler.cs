using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using BotFramework.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Base;

/// <summary>
/// Базовый класс обработчика состояния.
/// </summary>
/// <typeparam name="TResuorces">Тип класса ресурсов. Строки, пути к файлам и т.д.</typeparam>
public abstract class BaseBotPriorityHandler : ControllerBase, IBaseBotHandler
{
    /// <inheritdoc/>
    public BotUser User { get; set; }

    /// <inheritdoc/>
    public BotChat Chat { get; set; }
    
    /// <inheritdoc/>
    public Update Update { get; set; }

    /// <inheritdoc/>
    public BotDbContext BotDbContext { get; set; }

    /// <inheritdoc/>
    public string MediaDirectory { get; set; }

    /// <inheritdoc/>
    public ITelegramBotClient BotClient { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<ClaimValue> UserClaims { get; set; }

    public BaseBotPriorityHandler(IServiceProvider serviceProvider)
    {
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        BotDbContext = serviceProvider.GetRequiredService<BotDbContext>();
        var botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        MediaDirectory = botConfig.MediaDirectory;
    }

    /// <inheritdoc/>
    public abstract Task HandleBotRequest(Update update);
}