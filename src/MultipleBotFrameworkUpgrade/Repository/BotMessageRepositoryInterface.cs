﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MultipleBotFrameworkUpgrade.Db.Entity;

namespace MultipleBotFrameworkUpgrade.Repository;

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
    Task<List<BotUpdateEntity>> GetLastCountChatMessages(long botId, long chatEntityId, int count);
}