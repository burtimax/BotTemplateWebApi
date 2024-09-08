namespace MultipleTestBot.Endpoints.Common.Request;

public class PaginationRequest
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