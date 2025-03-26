using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Db.Entity;

/// <summary>
/// Сообщение пользователя
/// </summary>
[Comment("Таблица сохраненных сообщений бота. Для последующего использования")]
public class BotSavedMessageEntity : BaseEntity<long>
{
    /// <summary>
    /// Внешний ключ на сущность BotChat
    /// </summary>
    [Comment("Telegram ID чата.")]
    public long? TelegramChatId { get; set; }
        
    /// <summary>
    /// Внешний ключ на сущность BotChat
    /// </summary>
    [Comment("Telegram ID пользователя.")]
    public long? TelegramUserId { get; set; }
    
    /// <summary>
    /// Telegram Message Id
    /// </summary>
    [Comment("Идентификатор сообщения в Telegram чате.")]
    public int TelegramMessageId { get; set; }
    
    /// <summary>
    /// Медиа группа сообщения.
    /// </summary>
    [Comment("Медиа группа сообщения. Приадлежит ли сообщение медиа группе?")]
    public string? MediaGroupId { get; set; }
        
    /// <summary>
    /// Сериализованное в json сообщение. <see cref="Message"/>
    /// </summary>
    [Comment("Сериализованное в json сообщение.")]
    public string? TelegramMessageJson { get; set; }

    /// <summary>
    /// Комментарий к сохраненному сообщению.
    /// </summary>
    [Comment("Комментарий к сообщению.")]
    public string? Comment { get; set; }
    
    /// <summary>
    /// Внешний ключ на бота.
    /// </summary>
    [Comment("Внешний ключ на бота.")]
    public long BotId { get; set; }
    public BotEntity? Bot { get; set; }
    
    [NotMapped]
    public Message? TelegramMessage
    {
        get
        {
            if (string.IsNullOrEmpty(TelegramMessageJson)) return null;
            return JsonSerializer.Deserialize<Message>(TelegramMessageJson);
        }
        set => TelegramMessageJson = JsonSerializer.Serialize(value, new JsonSerializerOptions()
        {
            WriteIndented = true,
        });
    }
}