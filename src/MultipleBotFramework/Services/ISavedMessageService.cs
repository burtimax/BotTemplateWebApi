using System.Threading.Tasks;
using MultipleBotFramework.Db.Entity;
using Telegram.Bot.Types;

namespace MultipleBotFramework.Services;


/// <summary>
/// Сервис для работы с сущностями <see cref="BotSavedMessageEntity"/>.
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
    public Task<BotSavedMessageEntity> SaveMessageFromUpdate(long botId, BotChatEntity? chat, BotUserEntity? user, Message message);
    
    /// <summary>
    /// Есть ли в БД сохраненные сообщения, у которых GroupMediaId такой же.
    /// </summary>
    /// <param name="telegramChatId">ИД чата.</param>
    /// <param name="telegramUserId">ИД пользователя.</param>
    /// <param name="mediaGroupId">ИД медиа группы сообщений.</param>
    /// <returns></returns>
    public Task<bool> HasSavedMessageWithMediaType(long botId, long? telegramChatId, long? telegramUserId, string? mediaGroupId);
}