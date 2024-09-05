using System.Collections.Generic;

namespace MultipleBotFramework.Db.Entity;

public class BotEntity : BaseEntity<long>
{
    public string Token { get; set; }
    public string? Username { get; set; }
    public string? Description { get; set; }
    public string? Comment { get; set; }

    /// <summary>
    /// Навигационное поле.
    /// Пользователи бота.
    /// </summary>
    public List<BotUserEntity> Users { get; set; }
}