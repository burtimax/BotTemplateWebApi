using System.ComponentModel;

namespace MultipleBotFrameworkEndpoints.Models;

public class Pagination
{
    public static Pagination All = new Pagination()
    {
        PageNumber = 1,
        PageSize = int.MaxValue,
    };
    
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