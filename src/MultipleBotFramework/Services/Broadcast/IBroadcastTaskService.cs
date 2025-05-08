using System.Collections.Generic;
using System.Threading.Tasks;
using MultipleBotFramework.Db.BroadcastDb.Entity;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Services.Interfaces;

public interface IBroadcastTaskService
{
    public Task<BroadcastTask> GetNextBroadcastTask();
    public Task<List<BroadcastMessage>?> GetNextBroadcastTaskMessages(long broadcastTaskId, int takeCount = 1000);
    public Task StartBroadcastTask(long broadcastTaskId);
    public Task FinishBroadcastTask(long broadcastTaskId);
    public Task TerminateBroadcastTask(long broadcastTaskId);

    public Task<BroadcastTask?> NewBroadcastTask(string botToken, long botId, Message message,
        List<long> chatIds, ReplyMarkup? replyMarkup);
}