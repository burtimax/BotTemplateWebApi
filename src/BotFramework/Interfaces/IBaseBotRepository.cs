using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Models;
using TelegramModel = Telegram.Bot.Types;

namespace BotFramework.Interfaces
{
    public interface IBaseBotRepository
    {
        /// <summary>
        /// Has User with Id in Db?
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> IsUserInBot(long userId);

        /// <summary>
        /// Has Chat with Id in Db?
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        Task<bool> IsChatInBot(TelegramModel.ChatId chatId);

        /// <summary>
        /// Add Telegram type User to DB
        /// </summary>
        /// <param name="user">Telegram CurrentUser object</param>
        Task AddUserToBot(TelegramModel.User user, string role);

        /// <summary>
        /// Add Telegram type Chat to DB
        /// </summary>
        /// <param name="chat">Telegram Chat object</param>
        Task AddChatToBot(TelegramModel.Chat chat, BotUser chatOwner);

        /// <summary>
        /// Add Telegram Message type to DB
        /// </summary>
        /// <param name="mes">Telegram Message object</param>
        Task SaveMessage(InboxMessage message);

        /// <summary>
        /// Get User model data from DB
        /// </summary>
        /// <param name="userId">Id</param>
        /// <returns>User model</returns>
        Task<BotUser> GetUser(long userId);

        /// <summary>
        /// Get Chat model data from Db
        /// </summary>
        /// <param name="chatId">Id</param>
        /// <returns>Chat model</returns>
        Task<BotChat> GetChat(TelegramModel.ChatId chatId);

        /// <summary>
        /// Set Chat model data to Db (can change CurrentState and chatData)
        /// </summary>
        /// <param name="chat">Chat model object</param>
        Task SetChatData(BotChat chat);
    }
}
