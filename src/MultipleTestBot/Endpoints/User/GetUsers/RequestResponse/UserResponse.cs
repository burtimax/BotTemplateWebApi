namespace MultipleTestBot.Endpoints.User.GetUsers.RequestResponse;

public class UserResponse
{
    public long Id { get; set; }
    
    /// <summary>
    /// Telegram идентификатор пользователя.
    /// </summary>
    public long TelegramId { get; set; }
    
    /// <summary>
    /// Ник в Telegram.
    /// </summary>
    public string? TelegramUsername { get; set; }

    /// <summary>
    /// Телефон.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Роль пользователя (user, moderator, admin).
    /// </summary>
    public string Role { get; set; }
    
    /// <summary>
    /// Необязательно. <a href="https://en.wikipedia.org/wiki/IETF_language_tag">IETF language tag</a>
    /// Языковой код пользователя.
    /// </summary>
    public string? LanguageCode { get; set; }

    /// <summary>
    /// Флаг заблокированного пользователя.
    /// </summary>
    public bool IsBlocked { get; set; } = false;
    
    /// <summary>
    /// Имя пользователя в Telegram.
    /// </summary>
    public string? TelegramFirstname { get; set; }
    
    /// <summary>
    /// Фамилия пользователя в Telegram.
    /// </summary>
    public string? TelegramLastname { get; set; }

    /// <summary>
    /// Внешний ключ на бота.
    /// </summary>
    public long BotId { get; set; }
}