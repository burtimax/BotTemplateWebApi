using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Db.BroadcastDb.Entity;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Services.Interfaces;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Services;

public class BroadcastTaskService : IBroadcastTaskService
{
    private readonly BroadcastDbContext _db;

    public BroadcastTaskService(BroadcastDbContext db)
    {
        _db = db;
    }

    public async Task<BroadcastTask?> GetNextBroadcastTask()
    {
        return await _db.BroadcastTasks.Where(t => t.Status == BroadcastTaskStatus.New ||
                                             t.Status == BroadcastTaskStatus.InProcess && 
                                             t.StartAt >= DateTimeOffset.Now)
            .OrderBy(t => t.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<BroadcastMessage>?> GetNextBroadcastTaskMessages(long broadcastTaskId, int takeCount = 1000)
    {
        return await _db.BroadcastMessages.Where(t => t.BroadcastTaskId == broadcastTaskId &&
                                                t.UpdatedAt == null)
            .Take(takeCount)
            .ToListAsync();
    }

    public async Task StartBroadcastTask(long broadcastTaskId)
    {
        BroadcastTask task = await _db.BroadcastTasks.FirstAsync(t => t.Id == broadcastTaskId);
        task.Status = BroadcastTaskStatus.InProcess;
        _db.BroadcastTasks.Update(task);
        await _db.SaveChangesAsync();
    }
    
    public async Task FinishBroadcastTask(long broadcastTaskId)
    {
        BroadcastTask task = await _db.BroadcastTasks.FirstAsync(t => t.Id == broadcastTaskId);
        task.Status = BroadcastTaskStatus.Done;
        _db.BroadcastTasks.Update(task);
        await _db.SaveChangesAsync();
    }
    
    public async Task TerminateBroadcastTask(long broadcastTaskId)
    {
        BroadcastTask task = await _db.BroadcastTasks.FirstAsync(t => t.Id == broadcastTaskId);
        task.Status = BroadcastTaskStatus.Terminated;
        _db.BroadcastTasks.Update(task);
        await _db.SaveChangesAsync();
    }

    public async Task<BroadcastTask?> NewBroadcastTask(string botToken, long botId, Message message,
        List<long> chatIds, ReplyMarkup? replyMarkup, DateTimeOffset? startAt = null)
    {
        if (chatIds == null || chatIds.Any() == false) return null;
        List<BroadcastMessage> messages = chatIds.Distinct().Select(c => new BroadcastMessage()
        {
            ChatId = c
        }).ToList();
        
        BroadcastTask brTask = new()
        {
            BotToken = botToken,
            BotId = botId,
            Status = BroadcastTaskStatus.New,
            Text = message.Text ?? message.Caption,
            FromChatId = message.Chat.Id,
            FromMessageId = message.MessageId,
            MediaFileId = message.Photo?.Last()?.FileId ?? message.Video?.FileId ??
                message.Voice?.FileId ?? message.VideoNote?.FileId ?? message.Document?.FileId,
            ReplyMarkupJson = replyMarkup?.ToJson(),
            Messages = messages,
            StartAt = startAt ?? DateTimeOffset.Now,
        };
        
        _db.BroadcastTasks.Add(brTask);
        
        await _db.SaveChangesAsync();
        return brTask;
    }
}