using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using TelegramModel = Telegram.Bot.Types;

namespace BotFramework.Repository
{
    /// <summary>
    /// Основной репозиторий для работы с базовыми сущностями бота.
    /// Пользователями, чатами, сообщениями.
    /// </summary>
    public interface IBaseBotRepository
    {
        /// <summary>
        /// Получить пользователя по ИД.
        /// </summary>
        Task<BotUser?> GetUser(long userId);
        
        /// <summary>
        /// Получить пользователя по @username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<BotUser?> GetUser(string username);

        /// <summary>
        /// Добавить пользователя или обновить информацию по нему.
        /// Обновляет информацию (Firstname, Lastname, Username)
        /// Потому что с прошествием времени может поменяться эта информация.
        /// </summary>
        Task<BotUser> UpsertUser(TelegramModel.User user);

        /// <summary>
        /// Получить чат по ИД.
        /// </summary>
        Task<BotChat?> GetChat(TelegramModel.ChatId chatId);

        /// <summary>
        /// Получить чат пользователя.
        /// </summary>
        /// <param name="botUserId">ИД пользователя в системе</param>
        /// <returns></returns>
        Task<BotChat?> GetChat(long botUserId);
        
        /// <summary>
        /// Добавить чат пользователя.
        /// </summary>
        /// <param name="chat">Telegram Chat object</param>
        Task<BotChat> AddChat(TelegramModel.Chat chat, BotUser chatOwner);

        /// <summary>
        /// Добавить сообщение.
        /// </summary>
        Task<BotUpdate> AddUpdate(SaveUpdateDto updateDto);

        /// <summary>
        /// Получить разрешения пользователя.
        /// </summary>
        /// <param name="userId">ИД пользователя.</param>
        /// <returns>Список разрешений пользователя.</returns>
        Task<IEnumerable<BotClaim>?> GetUserClaims(long userId);
        
        /// <summary>
        /// Добавить разрешение пользователю.
        /// </summary>
        /// <remarks>
        /// Если разрешение уже есть, то добавлять не следует.
        /// </remarks>
        /// <param name="userId">ИД пользователя.</param>
        /// <param name="claim">Наименование разрешения.</param>
        /// <returns></returns>
        Task AddClaimToUser(long userId, string claim);
        
        /// <summary>
        /// Отменить разрешение у пользователя.
        /// </summary>
        /// <param name="userId">ИД пользователя.</param>
        /// <param name="claim">Наименование разрешения.</param>
        /// <returns></returns>
        Task RemoveClaimFromUser(long userId, string claim);

        /// <summary>
        /// Проверка доступности разрешений у пользователя.
        /// </summary>
        /// <param name="userId">ИД пользователя.</param>
        /// <param name="claims">Наименования разрешений.</param>
        /// <returns></returns>
        Task<bool> HasUserClaims(long userId, params string[] claims);

        /// <summary>
        /// Получить все разрешения имеющиеся у бота.
        /// </summary>
        public Task<IEnumerable<BotClaim>> GetAllClaims(bool hideBruceClaim = false);

        /// <summary>
        /// Получение claim по наименованию.
        /// </summary>
        /// <param name="name">Наименование клэйма.</param>
        /// <returns></returns>
        public Task<BotClaim?> GetClaimByName(string name);

        /// <summary>
        /// Получение claim по ИД.
        /// </summary>
        /// <param name="name">Идентификатор клэйма.</param>
        /// <returns></returns>
        public Task<BotClaim?> GetClaimById(long id);
    }
}