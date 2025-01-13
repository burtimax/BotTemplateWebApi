using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MultipleBotFramework.Models;
using MultipleBotFramework.Services.Interfaces;
using Quartz;
using Telegram.BotAPI;

namespace MultipleBotFramework.Quartz.Jobs.Notification;

public class BotNotificationJob : IJob
{
    public static readonly JobKey Key = new JobKey("bot-notification-job", "bot");
    private static bool IsWorkingNow = false;
    internal static Queue<BotNotificationTask> NotificationTasks = new();
    
    private readonly IBotFactory _botFactory;
    
    public BotNotificationJob(IBotFactory botFactory)
    {
        _botFactory = botFactory;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        if (IsWorkingNow == true) return;
        IsWorkingNow = true;

        try
        {
            await PerformJob(context);
        }
        catch (Exception e)
        {
            throw;
        }
        finally
        {
            IsWorkingNow = false;
        }
    }

    private async Task PerformJob(IJobExecutionContext context)
    {
        int delay = 200;
        
        if(NotificationTasks.Any() == false) return;
        BotNotificationTask notifTask = NotificationTasks.Dequeue();

        ITelegramBotClient? botclient = await _botFactory.GetInstance(notifTask.BotId);

        if (botclient == null) throw new Exception($"Not found bot by id [{notifTask.BotId}]");

        if (notifTask.Action == null 
            || notifTask.ChatIds == null 
            || notifTask.ChatIds.Any() == false) return;
        
        foreach (var chatId in notifTask.ChatIds)
        {
            try
            {
                await notifTask.Action.Invoke(botclient, chatId);
                await Task.Delay(delay);
            }
            catch (Exception e)
            {
                // ToDo Бот не смог отправить уведомление, обработать.
                // throw;
            }
        }
    }
}