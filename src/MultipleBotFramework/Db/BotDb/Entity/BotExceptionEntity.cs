using System;
using Microsoft.EntityFrameworkCore;

namespace MultipleBotFramework.Db.Entity;

/// <summary>
/// Сообщения об ошибке в боте.
/// </summary>
[Comment("Сообщения об ошибке в боте.")]
public class BotExceptionEntity : BaseEntity<long>
{
    /// <summary>
    /// Кто отправил запрос боту, когда произошла ошибка.
    /// </summary>
    [Comment("Кто отправил запрос боту, когда произошла ошибка.")]
    public long? UserEntityId { get; set; }
    public BotUserEntity? UserEntity { get; set; }

    /// <summary>
    /// ИД чата, откуда пришел запрос, когда произошла ошибка. 
    /// </summary>
    [Comment("ИД чата, откуда пришел запрос, когда произошла ошибка.")]
    public long? ChatEntityId { get; set; }
    public BotChatEntity? ChatEntity { get; set; }
    
    /// <summary>
    /// ИД запроса, в момент обработки которого произошла ошибка. 
    /// </summary>
    [Comment("ИД запроса, в момент обработки которого произошла ошибка.")]
    public long? UpdateEntityId { get; set; }
    public BotUpdateEntity? UpdateEntity { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    [Comment("Сообщение об ошибке.")]
    public string? ExceptionMessage { get; set; }
    
    /// <summary>
    /// Стек вызовов в приложении, перед ошибкой.
    /// </summary>
    [Comment("Стек вызовов в приложении, перед ошибкой.")]
    public string? StackTrace { get; set; }
    
    /// <summary>
    /// Отчет об ошибке.
    /// </summary>
    [Comment("Отчет об ошибке.")]
    public string? ReportDescription { get; set; }
    
    /// <summary>
    /// Имя файла, в котором записан отчет об ошибке.
    /// </summary>
    [Comment("Имя файла, в котором записан отчет об ошибке.")]
    public string? ReportFileName { get; set; }
    
    /// <summary>
    /// Внешний ключ на бота.
    /// </summary>
    [Comment("Внешний ключ на бота.")]
    public long? BotId { get; set; }
    public BotEntity? Bot { get; set; }
}