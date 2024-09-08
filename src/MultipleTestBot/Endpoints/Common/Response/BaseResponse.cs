namespace MultipleTestBot.Endpoints.Common.Response;

/// <summary>
/// Базовая модель ответа.
/// </summary>
/// <typeparam name="T">Тип содержимого ответа.</typeparam>
public class BaseResponse<T> where T : class, new()
{
    /// <summary>
    /// Содержимое ответа.
    /// </summary>
    public T Content { get; set; }

    /// <summary>
    /// true - запрос завершился с ошибкой, иначе - false.
    /// </summary>
    public bool IsError { get; set; }

    /// <summary>
    /// Сообщение об ошибке.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Конструктор <see><cref>BaseResponse</cref></see>.
    /// </summary>
    public BaseResponse()
    {
        Content = new T();
        Message = string.Empty;
    }

    /// <summary>
    /// Конструктор <see><cref>BaseResponse</cref></see>.
    /// </summary>
    public BaseResponse(T content, bool isError, string message)
    {
        Content = content;
        IsError = isError;
        Message = message;
    }

    /// <summary>
    /// Конструктор <see><cref>BaseResponse</cref></see>.
    /// </summary>
    public BaseResponse(T content, string message)
    {
        Content = content;
        Message = message;
    }

    /// <summary>
    /// Конструктор <see><cref>BaseResponse</cref></see>.
    /// </summary>
    public BaseResponse(T content)
    {
        Content = content;
        Message = string.Empty;
    }
}