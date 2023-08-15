using Newtonsoft.Json.Serialization;

namespace BotFramework.Options;

public class BotConfiguration
{
    /// <summary>
    /// Токен бота из Telegram.org.
    /// </summary>
    public string TelegramToken { get; set; }
        
    /// <summary>
    /// URL адрес, на который будут отправляться запросы к боту для обработки.
    /// </summary>
    public string Webhook { get; set; }

    /// <summary>
    /// Файл ресурсов бота
    /// </summary>
    public string? ResourcesFile { get; set; }
    
    /// <summary>
    /// Строка подключения к БД бота.
    /// </summary>
    public string DbConnection { get; set; }
    
    /// <summary>
    /// Пароль для команд бота.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Директория для хранения медиа файлов бота.
    /// </summary>
    public string MediaPath { get; set; }
}