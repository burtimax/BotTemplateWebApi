using System.Threading.Tasks;
using BotFramework.Db.Entity;
using Telegram.Bot.Types;

namespace BotFramework.Services;


/// <summary>
/// Сервис для работы с сущностями <see cref="BotSavedMessage"/>.
/// </summary>
public interface ISavedMessageService
{
    /// <summary>
    /// Добавить в сохраненные сообщения бота.
    /// </summary>
    /// <param name="chat">Чат.</param>
    /// <param name="user">Пользователь.</param>
    /// <param name="update">Сообщение.</param>
    /// <returns></returns>
    public Task<BotSavedMessage> SaveMessageFromUpdate(BotChat? chat, BotUser? user, Message message);
    
    /// <summary>
    /// Есть ли в БД сохраненные сообщения, у которых GroupMediaId такой же.
    /// </summary>
    /// <param name="telegramChatId">ИД чата.</param>
    /// <param name="telegramUserId">ИД пользователя.</param>
    /// <param name="mediaGroupId">ИД медиа группы сообщений.</param>
    /// <returns></returns>
    public Task<bool> HasSavedMessageWithMediaType(long? telegramChatId, long? telegramUserId, string? mediaGroupId);
}