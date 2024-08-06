namespace MultipleBotFrameworkUpgrade.Options;

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
    /// Ограничение по кол-ву запросов у бота.
    /// </summary>
    public int? BoundRequestsInSecond { get; set; } 
}