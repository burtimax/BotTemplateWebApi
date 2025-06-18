/// <summary>
/// Модель для пагинации результатов запросов.
/// Используется для разбиения больших наборов данных на страницы.
/// </summary>

using System.ComponentModel;

namespace MultipleBotFrameworkEndpoints.Models;

public class Pagination
{
    /// <summary>
    /// Статический экземпляр для получения всех элементов без пагинации
    /// </summary>
    public static Pagination All = new Pagination()
    {
        PageNumber = 1,
        PageSize = int.MaxValue,
    };
    
    /// <summary>
    /// Номер текущей страницы.
    /// По умолчанию: 1
    /// </summary>
    [DefaultValue(1)]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Количество элементов на одной странице.
    /// По умолчанию: 20
    /// </summary>
    [DefaultValue(20)]
    public int PageSize { get; set; } = 20;
}