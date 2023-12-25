using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Options;

public class BotOptions
{
    /// <summary>
    /// Нужно ли делать сохранение истории запросов к боту в БД.
    /// </summary>
    public bool SaveUpdatesInDatabase { get; set; } = true;
    
    /// <summary>
    /// Нужно ли сохранять ошибки в БД.
    /// </summary>
    public bool SaveExceptionsInDatabase { get; set; } = true;
    
    /// <summary>
    /// Нужно ли отчеты по ошибкам в директорию.
    /// </summary>
    public bool SaveExceptionsInDirectory { get; set; } = true;
    
    /// <summary>
    /// Задаем обработчик для Poll запроса.
    /// </summary>
    public BotHandlerDelegates.BotPollHandler PollHandler { get; set; }
    
    /// <summary>
    /// Задаем обработчик для запроса удаления бота у пользователя
    /// </summary>
    public BotHandlerDelegates.BotDeleteHandler BotDeleteHandler { get; set; }
}