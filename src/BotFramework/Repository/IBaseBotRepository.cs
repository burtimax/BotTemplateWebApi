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
        Task<BotUser> GetUser(long userId);

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
        /// Добавить чат пользователя.
        /// </summary>
        /// <param name="chat">Telegram Chat object</param>
        Task<BotChat> AddChat(TelegramModel.Chat chat, BotUser chatOwner);

        /// <summary>
        /// Добавить сообщение.
        /// </summary>
        Task<BotMessage> AddMessage(SaveMessageDto messageDto);
    }
}