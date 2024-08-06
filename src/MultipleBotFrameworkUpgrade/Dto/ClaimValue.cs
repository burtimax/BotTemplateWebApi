namespace MultipleBotFrameworkUpgrade.Dto;

/// <summary>
/// Данные по клэйму.
/// </summary>
public class ClaimValue
{
    /// <summary>
    /// ИД клэйма.
    /// </summary>
    public long Id { get; set; }
    
    /// <summary>
    /// Строковый идентификатор клэйма.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Описание клэйма.
    /// </summary>
    public string Description { get; set; }
    
    public ClaimValue(string name, string description)
    {
        Name = name;
        Description = description;
    }
    
    public ClaimValue(long id, string name, string description)
        : this(name, description)
    {
        Id = id;
    }
}