using System;
using System.Text.Json;
using System.Text.RegularExpressions;
using MultipleBotFrameworkUpgrade.Utils;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFrameworkUpgrade.Extensions;

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
}
