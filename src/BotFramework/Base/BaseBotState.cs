using System;
using System.Dynamic;
using System.Threading.Tasks;
using BotFramework.Base;
using BotFramework.Db.Entity;
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
    public BotUser User { get; set; }
    public BotChat Chat { get; set; }
    public ITelegramBotClient BotClient { get; set; }

    public BaseBotState(IServiceProvider serviceProvider)
    {
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
    }

    public abstract Task HandleBotRequest(Update update);
   
}