using System;
using BotFramework.Db.Entity;
using Telegram.Bot.Types;

namespace BotFramework.Utils.ExceptionHandler;

public class BotExceptionHandlerArgs
{
    public IServiceProvider ServiceProvider { get; set; }
    public Exception Exception { get; set; }
    public Update? TelegramUpdate { get; set; }
    public BotUpdate? BotUpdate { get; set; }
    public BotUser? BotUser { get; set; }
    public BotChat? BotChat { get; set; }

    public BotExceptionHandlerArgs(Exception exception, IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Exception = exception;
    }
}