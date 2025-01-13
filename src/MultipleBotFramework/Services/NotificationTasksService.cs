using System.Threading.Tasks;
using MultipleBotFramework.Quartz.Jobs.Notification;
using MultipleBotFramework.Services.Interfaces;

namespace MultipleBotFramework.Services;

/// <inheritdoc />
public class BotNotificationTasksService : IBotNotificationTasksService
{
    /// <inheritdoc />
    public Task AddNotificationTask(BotNotificationTask botNotificationTask)
    {
        BotNotificationJob.NotificationTasks.Enqueue(botNotificationTask);
        return Task.CompletedTask;
    }
}