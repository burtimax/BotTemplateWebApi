using System.Threading.Tasks;
using MultipleBotFramework.Quartz.Jobs.Notification;

namespace MultipleBotFramework.Services.Interfaces;

/// <summary>
/// Сервис для управления задачами рассылки сообщений.
/// </summary>
public interface IBotNotificationTasksService
{
    /// <summary>
    /// Добавить в очередь на рассылку.
    /// </summary>
    /// <param name="botNotificationTask"></param>
    /// <returns></returns>
    Task AddNotificationTask(BotNotificationTask botNotificationTask);
}