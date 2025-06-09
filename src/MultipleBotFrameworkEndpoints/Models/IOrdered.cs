/// <summary>
/// Интерфейс для поддержки сортировки в запросах.
/// Позволяет указать порядок сортировки для результатов запроса.
/// </summary>

namespace MultipleBotFrameworkEndpoints.Models;

public interface IOrdered
{
    /// <summary>
    /// Строка, определяющая порядок сортировки.
    /// </summary>
    /// <remarks>
    /// Формат строки: "+PropertyName1,-PropertyName2,+PropertyName3"
    /// где:
    /// - "+" означает сортировку по возрастанию
    /// - "-" означает сортировку по убыванию
    /// - PropertyName - имя свойства для сортировки
    /// </remarks>
    public string? Order { get; set; }
}