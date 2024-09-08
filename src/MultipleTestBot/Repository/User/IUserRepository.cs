using MultipleBotFramework.Db.Entity;
using MultipleTestBot.Models;
using MultipleTestBot.Repository.User.Dto;

namespace MultipleTestBot.Repository.User;

/// <summary>
/// Репозиторий для сущности пользователя.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получить пользователей.
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    Task<PagedList<BotUserEntity>> GetUsers(GetUsersDto dto);
}