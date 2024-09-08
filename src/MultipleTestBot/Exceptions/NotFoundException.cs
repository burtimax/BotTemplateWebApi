namespace MultipleTestBot.Exceptions;

/// <summary>
/// Исключение "НЕ НАЙДЕНО".
/// </summary>
public class NotFoundException : Exception
{
    /// <inheritdoc/>
    public NotFoundException() : base("Сущность не найдена")
    {
    }
    
    /// <inheritdoc/>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <inheritdoc/>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}