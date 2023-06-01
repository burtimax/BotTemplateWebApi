using System.Text.RegularExpressions;

namespace BotFramework.Extensions;

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
}
