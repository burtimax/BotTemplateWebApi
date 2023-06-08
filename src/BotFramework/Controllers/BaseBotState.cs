using System;
using System.Dynamic;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Controllers;

public abstract class BaseBotState : ControllerBase
{
    public BotUser User;
    public BotChat Chat;
    protected ITelegramBotClient BotClient;

    public BaseBotState(IServiceProvider serviceProvider)
    {
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
    }

    public abstract Task HandleBotRequest(Update update);
}