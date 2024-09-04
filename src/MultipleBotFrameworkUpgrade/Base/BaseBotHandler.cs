using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dto;
using MultipleBotFrameworkUpgrade.Models;
using MultipleBotFrameworkUpgrade.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.Base;

/// <summary>
/// Базовый обработчик запроса бота.
/// </summary>
/// <typeparam name="TResuorces">Тип класса ресурсов. Строки, пути к файлам и т.д.</typeparam>
public abstract class BaseBotHandler : ControllerBase, IBaseBotHandler
{
    /// <inheritdoc />
    public IServiceProvider ServiceProvider { get; set; }

    /// <inheritdoc />
    public long BotId { get; set; }

    /// <inheritdoc/>
    public BotUserEntity? User { get; set; }

    /// <inheritdoc/>
    public BotChatEntity? Chat { get; set; }
    
    /// <inheritdoc/>
    public Update Update { get; set; }

    /// <inheritdoc/>
    public BotDbContext BotDbContext { get; set; }

    /// <inheritdoc/>
    public string MediaDirectory { get; set; }

    /// <inheritdoc/>
    public ITelegramBotClient BotClient { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<ClaimValue>? UserClaims { get; set; }

    /// <inheritdoc />
    public bool IsOwner { get; set; }

    public BaseBotHandler(IServiceProvider serviceProvider)
    {
        BotDbContext = serviceProvider.GetRequiredService<BotDbContext>();
        var botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        MediaDirectory = botConfig.MediaDirectory;
    }

    /// <inheritdoc/>
    public abstract Task HandleBotRequest(Update update);

    /// <inheritdoc />
    public virtual Task SendIntroduction()
    {
        return Task.CompletedTask;
    }
}