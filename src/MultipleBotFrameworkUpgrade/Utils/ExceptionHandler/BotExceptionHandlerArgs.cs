using System;
using MultipleBotFrameworkUpgrade.Db.Entity;
using Telegram.BotAPI.GettingUpdates;


namespace MultipleBotFrameworkUpgrade.Utils.ExceptionHandler;

public class BotExceptionHandlerArgs
{
    public long BotId { get; set; }
    public IServiceProvider ServiceProvider { get; set; }
    public Exception Exception { get; set; }
    public Update? TelegramUpdate { get; set; }
    public BotUpdateEntity? BotUpdate { get; set; }
    public BotUserEntity? BotUser { get; set; }
    public BotChatEntity? BotChat { get; set; }

    public BotExceptionHandlerArgs(Exception exception, IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        Exception = exception;
    }
}