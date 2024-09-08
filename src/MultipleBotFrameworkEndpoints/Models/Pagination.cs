using System.ComponentModel;

namespace MultipleBotFrameworkEndpoints.Models;

public abstract class Pagination
{
    /// <summary>
    /// Номер страницы.
    /// </summary>
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Кол-во элементов на странице.
    /// </summary>
    [DefaultValue(20)]
    public int PageSize { get; set; } = 20;
}
