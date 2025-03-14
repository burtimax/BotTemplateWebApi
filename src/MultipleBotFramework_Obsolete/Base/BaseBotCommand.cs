﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework_Obsolete.Db;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Dto;
using MultipleBotFramework_Obsolete.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MultipleBotFramework_Obsolete.Base;

/// <summary>
/// Базовый класс обработчика команды.
/// </summary>
public abstract class BaseBotCommand : ControllerBase, IBaseBotHandler
{
    /// <inheritdoc />
    public long BotId { get; set; }

    /// <inheritdoc/>
    public BotUserEntity User { get; set; }
    
    /// <inheritdoc/>
    public BotChatEntity Chat { get; set; }
    
    /// <inheritdoc/>
    public Update Update { get; set; }

    /// <inheritdoc/>
    public BotDbContext BotDbContext { get; set; }
    
    /// <inheritdoc/>
    public string MediaDirectory { get; set; }
    
    /// <inheritdoc/>
    public ITelegramBotClient BotClient { get; set; }

    public BaseBotCommand(IServiceProvider serviceProvider)
    {
        BotDbContext = serviceProvider.GetRequiredService<BotDbContext>();
        var botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        MediaDirectory = botConfig.MediaDirectory;
    }

    public IReadOnlyList<ClaimValue> UserClaims { get; set; }

    /// <inheritdoc />
    public bool IsOwner { get; set; }

    /// <inheritdoc/>
    public abstract Task HandleBotRequest(Update update);
    
}