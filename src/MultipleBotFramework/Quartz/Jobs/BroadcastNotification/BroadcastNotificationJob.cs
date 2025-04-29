using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Db.BroadcastDb.Entity;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils.Keyboard;
using Quartz;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Quartz.Jobs.BroadcastNotification;

public class BroadcastNotificationJob : IJob
{
    private const int NotificationInterval = 5;
    private const int NotificationCountIfNoBroadcast = 20;
    
    
    public static readonly JobKey Key = new JobKey("bot-notification-job", "bot");
    private static bool IsWorkingNow = false;

    private readonly BroadcastConfiguration _config;
    private readonly IBroadcastTaskService _broadcastTaskService;
    private readonly IBotNotificationService _botNotificationService;
    private readonly BroadcastDbContext _db;
    private readonly BotDbContext _botDb;
    
    public BroadcastNotificationJob(BroadcastConfiguration configuration,
        IBroadcastTaskService broadcastTaskService, BroadcastDbContext db, BotDbContext botDb, IBotNotificationService botNotificationService)
    {
        _broadcastTaskService = broadcastTaskService;
        _db = db;
        _botDb = botDb;
        _botNotificationService = botNotificationService;
        _config = configuration;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        if (IsWorkingNow == true) return;
        IsWorkingNow = true;

        try
        {
            await PerformJob(context, context.CancellationToken);
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

    private async Task PerformJob(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        BroadcastTask broadcastTask = await _broadcastTaskService.GetNextBroadcastTask();
        if (broadcastTask == null)
        {
            await SendMultipleNotifications();
            return;
        }
        
        await _broadcastTaskService.StartBroadcastTask(broadcastTask.Id);
        
        ITelegramBotClient botClient = new TelegramBotClient(broadcastTask.BotToken);
        
        InlineKeyboardMarkup? reply = string.IsNullOrEmpty(broadcastTask.ReplyMarkupJson) ? null : broadcastTask.ReplyMarkupJson.FromJson<InlineKeyboardMarkup>(); 
        
        await botClient.SendMessageAsync(broadcastTask.FromChatId, $"Бот начинает рассылку [{broadcastTask.Id}]");
        await botClient.CopyMessageAsync(broadcastTask.FromChatId, broadcastTask.FromChatId, broadcastTask.FromMessageId, replyMarkup:reply);

        int c = 1;
        while(true)
        {
            var messages = await _broadcastTaskService.GetNextBroadcastTaskMessages(broadcastTask.Id);
            if (messages == null || messages.Any() == false) break;
            
            foreach (var mes in messages)
            {
                if (cancellationToken.IsCancellationRequested == true) break;

                if (c % NotificationInterval == 0)
                {
                    var res = await _botNotificationService.SendNextNotification();
                }
                
                try
                {
                    await botClient.CopyMessageAsync(mes.ChatId, broadcastTask.FromChatId, broadcastTask.FromMessageId, replyMarkup: reply);
                    mes.IsSuccess = true;
                }
                catch (Exception e)
                {
                    mes.IsSuccess = false;
                    mes.ErrorLog = e.Message;

                    await ProcessErrorIfNeed(broadcastTask.BotId, mes, e);
                }
                finally
                {
                    _db.BroadcastMessages.Update(mes);
                    c++;
                    await Task.Delay(_config.MessageDelayMilliseconds);
                }
            }
            
            await _db.SaveChangesAsync();
        }
        
        await _broadcastTaskService.FinishBroadcastTask(broadcastTask.Id);
        await botClient.SendMessageAsync(broadcastTask.FromChatId, $"Бот заканчивает рассылку [{broadcastTask.Id}]");
    }

    private async Task SendMultipleNotifications()
    {
        int i = 0;
        while (i < NotificationCountIfNoBroadcast)
        {
            var res = await _botNotificationService.SendNextNotification();
            if(res == BotNotificationService.SendNotificationResult.InternalException) continue;
            if (res == BotNotificationService.SendNotificationResult.Nothing) break;
            i++;
            await Task.Delay(_config.MessageDelayMilliseconds);
        }
    }
    
    /// <summary>
    /// Обработка ошибки.
    /// </summary>
    /// <param name="mes"></param>
    /// <param name="e"></param>
    private async Task ProcessErrorIfNeed(long botId, BroadcastMessage mes, Exception e)
    {
        // Если не достучались до чата, значит уже не достучимся.
        if (e.Message == "Bad Request: chat not found")
        {
            BotChatEntity? chat = await _botDb.Chats
                .FirstOrDefaultAsync(x => x.Id == mes.ChatId && x.BotId == botId);
            if (chat != null)
            {
                chat.Status = "kicked";
                _botDb.Update(chat);
                await _botDb.SaveChangesAsync();
            }
        }
    }
    
}