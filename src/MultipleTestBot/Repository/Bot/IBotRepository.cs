using MultipleBotFramework.Db.Entity;
using MultipleTestBot.Models;
using MultipleTestBot.Repository.Bot.Dto;

namespace MultipleTestBot.Repository.Bot;

/// <summary>
/// Репозиторий для сущности бота.
/// </summary>
public interface IBotRepository
{
    /// <summary>
    /// Получить список ботов.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<PagedList<BotEntity>> GetBots(GetBotsDto dto);
}