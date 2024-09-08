using System.Threading.Tasks;
using MultipleBotFramework.Db.Entity;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Services.Interfaces;

/// <summary>
/// Сервис сохранения запросов для бота в истории.
/// </summary>
public interface ISaveUpdateService
{
    /// <summary>
    /// Сохранить запрос в истории.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="chat"></param>
    /// <param name="update"></param>
    /// <returns></returns>
    public Task<BotUpdateEntity> SaveUpdateInBotHistory(long botId, BotUserEntity? user, BotChatEntity? chat, Update update);

    // Сохраняем результат запроса бота в историю.
    public Task<BotUpdateEntity> SaveBotRequestResult<TResult>(string method, TResult result);
}