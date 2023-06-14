using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using Telegram.Bot.Types;

namespace BotFramework.Repository;

/// <summary>
/// Репозиторий сообщений бота.
/// </summary>
public interface IBotUpdateRepository
{
    /// <summary>
    /// Получить историю последних сообщений чата в определенном количестве.
    /// </summary>
    /// <param name="chatId">Идентификатор чата в Telegram.</param>
    /// <param name="count">Какое количество вернуть.</param>
    /// <returns></returns>
    Task<List<BotUpdate>> GetLastCountChatMessages(long chatEntityId, int count);
}