namespace MultipleTestBot.Models;

public abstract class Pagination
{
    /// <summary>
    /// Номер страницы.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Кол-во элементов на странице.
    /// </summary>
    public int PageSize { get; set; } = 20;
}
