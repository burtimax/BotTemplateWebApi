namespace BotFramework.Options;

public class BotOptions
{
    /// <summary>
    /// Нужно ли делать сохранение истории запросов к боту в БД.
    /// </summary>
    public bool SaveUpdatesInDatabase { get; set; } = true;
}