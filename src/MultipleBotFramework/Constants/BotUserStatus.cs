namespace MultipleBotFramework.Constants;

public class BotUserStatus
{
    /// <summary>
    /// Участник чата.
    /// </summary>
    public const string Member = "member";
    
    /// <summary>
    /// Пользователь заблокирован, заблокировал бота.
    /// </summary>
    public const string Banned = "kicked";
    
    /// <summary>
    /// Стал администратором.
    /// </summary>
    public const string Administrator = "administrator";
    
    /// <summary>
    /// Стал владельцем.
    /// </summary>
    public const string Owner = "creator";
    
    /// <summary>
    /// Получил ограниченные права. (Имеет какие-то ограничения).
    /// </summary>
    public const string Restricted = "restricted";
    
    /// <summary>
    /// Пользователь покинул чат/группу, но может просматривать его содержимое.
    /// </summary>
    public const string Left = "left";
    
}