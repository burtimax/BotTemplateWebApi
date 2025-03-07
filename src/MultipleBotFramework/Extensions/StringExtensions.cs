using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MultipleBotFramework.Extensions;

/// <summary>
/// Расширения для строк.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Расширения для перевода строки в snake_case.
    /// </summary>
    /// <param name="input">Строка для преобразования.</param>
    /// <returns>Строка в snake_case.</returns>
    public static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var startUnderscores = Regex.Match(input, @"^_+");
        return startUnderscores + Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    // TODO REFACTOR
    /// <summary>
    /// Из строки сделать клавиатуру для бота.
    /// </summary>
    /// <remarks>
    /// Пример валидной строки:
    /// [Кнопка 1#callback_1] [Кнопка 2#callback2] [Кнопка 3] \n [Кнопка 4] [Кнопка 5] [Кнопка 6]
    /// </remarks>
    /// <param name="str"></param>
    /// <typeparam name="T"><see cref="IReplyMarkup"/></typeparam>
    /// <returns></returns>
    // public static T ToReplyMarkup<T>(this string str) where T : ReplyMarkup
    // {
    //     StringMarkupBuilder<T> smb = new();
    //     return smb.Build(str);
    // }
    
    public static TResult? To<TResult>(this string json)
    {
        return JsonSerializer.Deserialize<TResult>(json);
    }
    
    /// <summary>
    /// Превращает строку в паттерн поиска.
    /// </summary>
    /// <remarks>
    /// Из строки [как дела] сделает строку [%как%дела%].
    /// </remarks>
    /// <param name="input"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string ToILikePattern(this string input, ILikePatternType type = ILikePatternType.StartsWith)
    {
        input = input.Trim(' ');

        if (string.IsNullOrEmpty(input)) return "";
        
        var words = input.Split(' ');
        StringBuilder sb = new();
        
        sb.Append("%");
        foreach (var word in words)
        {
            sb.Append(word + "%");
        }

        string resultPattern = sb.ToString();

        switch (type)
        {
            case ILikePatternType.StartsWith:
                return resultPattern.TrimStart('%');
            case ILikePatternType.EndsWith:
                return resultPattern.TrimEnd('%');
            default: return resultPattern;
        }
        
    }

    public enum ILikePatternType
    {
        StartsWith = 1,
        EndsWith = 2,
        Contains = 3,
    }
}
