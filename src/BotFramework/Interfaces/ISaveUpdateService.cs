using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using Telegram.Bot.Types;

namespace BotFramework.Interfaces;

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
    public Task<BotUpdate> SaveUpdateInBotHistory(BotUser user, BotChat chat, Update update);
}