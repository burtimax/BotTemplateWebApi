﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Dto;
using TelegramModel = Telegram.Bot.Types;

namespace MultipleBotFramework_Obsolete.Repository
{
    /// <summary>
    /// Основной репозиторий для работы с базовыми сущностями бота.
    /// Пользователями, чатами, сообщениями.
    /// </summary>
    public interface IBaseBotRepository
    {
        /// <summary>
        /// Получить пользователя по ИД Telegram.
        /// </summary>
        Task<BotUserEntity?> GetUser(long botId, long userTelegramId);

        /// <summary>
        /// Получить пользователя по @username.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<BotUserEntity?> GetUser(long botId, string username);

        /// <summary>
        /// Поиск пользователей по строке.
        /// </summary>
        /// <param name="searchStr">строка поиска.</param>
        /// <param name="skip">Параметр пагинации.</param>
        /// <param name="limit">Параметр пагинации.</param>
        /// <returns>Пользователи удовлетворяющие строке поиска.</returns>
        Task<IEnumerable<BotUserEntity>?> SearchUsers(long botId, string searchStr, int skip, int limit);
        
        /// <summary>
        /// Получить пользователя по @username или по ИД.
        /// </summary>
        /// <param name="userIdentity">Строковое представление @username или по ИД.</param>
        /// <returns>Пользователь.</returns>
        Task<BotUserEntity?> GetUserByIdentity(long botId, string userIdentity);

        /// <summary>
        /// Заблокировать пользователей.
        /// Установить значение параметра <see cref="BotUserEntity.IsBlocked"/> true.
        /// </summary>
        /// <param name="userIds">Список ИД пользователей.</param>
        /// <returns></returns>
        Task BlockUsers(long botId, params long[] userIds);
        
        /// <summary>
        /// Разблокировать пользователей.
        /// Установить значение параметра <see cref="BotUserEntity.IsBlocked"/> false.
        /// </summary>
        /// <param name="userIds">Список ИД пользователей.</param>
        /// <returns></returns>
        Task UnblockUsers(long botId, params long[] userIds);

        /// <summary>
        /// Получить пользователей, у которых есть разрешение.
        /// </summary>
        /// <param name="claimName">Наименование разрешения.</param>
        /// <returns>Список пользователей.</returns>
        Task<IEnumerable<BotUserEntity>> GetUsersByClaim(long botId, string claimName);
        
        /// <summary>
        /// Добавить пользователя или обновить информацию по нему.
        /// Обновляет информацию (Firstname, Lastname, Username)
        /// Потому что с прошествием времени может поменяться эта информация.
        /// </summary>
        Task<BotUserEntity?> UpsertUser(long botId, TelegramModel.User? user);

        /// <summary>
        /// Получить чат по ИД.
        /// </summary>
        Task<BotChatEntity?> GetChat(long botId, TelegramModel.ChatId chatId, long botUserId);
        
        /// <summary>
        /// Добавить чат пользователя.
        /// </summary>
        /// <param name="chat">Telegram Chat object</param>
        Task<BotChatEntity?> AddChat(long botId, TelegramModel.Chat? chat, BotUserEntity? chatOwner);

        /// <summary>
        /// Получить пользователя по ИД.
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Task<BotUserEntity?> GetUserById(long botId, long userId);

        /// <summary>
        /// Проверка, является ли пользователь владельцем бота.
        /// </summary>
        /// <param name="botId"></param>
        /// <param name="userTelegramId"></param>
        /// <returns></returns>
        public Task<bool> IsUserOwner(long botId, long userTelegramId);
        
        /// <summary>
        /// Добавить сообщение.
        /// </summary>
        Task<BotUpdateEntity> AddUpdate(long botId, SaveUpdateDto updateDto);

        /// <summary>
        /// Получить разрешения пользователя.
        /// </summary>
        /// <param name="userId">ИД пользователя.</param>
        /// <returns>Список разрешений пользователя.</returns>
        Task<IEnumerable<BotClaimEntity>?> GetUserClaims(long botId, long userId);
        
        /// <summary>
        /// Добавить разрешение пользователю.
        /// </summary>
        /// <remarks>
        /// Если разрешение уже есть, то добавлять не следует.
        /// </remarks>
        /// <param name="userId">ИД пользователя.</param>
        /// <param name="claim">Наименование разрешения.</param>
        /// <returns></returns>
        Task AddClaimToUser(long botId, long userId, string claim);
        
        /// <summary>
        /// Отменить разрешение у пользователя.
        /// </summary>
        /// <param name="userId">ИД пользователя.</param>
        /// <param name="claim">Наименование разрешения.</param>
        /// <returns></returns>
        Task RemoveClaimFromUser(long botId, long userId, string claim);

        /// <summary>
        /// Проверка доступности разрешений у пользователя.
        /// </summary>
        /// <param name="userId">ИД пользователя.</param>
        /// <param name="claims">Наименования разрешений.</param>
        /// <returns></returns>
        Task<bool> HasUserClaims(long botId, long userId, params string[] claims);

        /// <summary>
        /// Получить все разрешения имеющиеся у бота.
        /// </summary>
        public Task<IEnumerable<BotClaimEntity>> GetAllClaims(bool hideBruceClaim = false);

        /// <summary>
        /// Получение claim по наименованию.
        /// </summary>
        /// <param name="name">Наименование клэйма.</param>
        /// <returns></returns>
        public Task<BotClaimEntity?> GetClaimByName(string name);

        /// <summary>
        /// Получение claim по ИД.
        /// </summary>
        /// <param name="name">Идентификатор клэйма.</param>
        /// <returns></returns>
        public Task<BotClaimEntity?> GetClaimById(long id);
    }
}