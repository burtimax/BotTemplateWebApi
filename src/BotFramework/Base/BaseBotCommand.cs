using System;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Base;

/// <summary>
/// Базовый класс обработчика команды.
/// </summary>
public abstract class BaseBotCommand : ControllerBase, IBaseBotHandler
{
    public BotUser User { get; set; }
    public BotChat Chat { get; set; }
    public ITelegramBotClient BotClient { get; set; }

    public BaseBotCommand(IServiceProvider serviceProvider)
    {
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
    }

    public abstract Task HandleBotRequest(Update update);
    
}