// TODO REFACTOR
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.RegularExpressions;
// using MultipleBotFrameworkUpgrade.Exceptions;
// using Telegram.Bot.Types.ReplyMarkups;
// using Telegram.BotAPI.AvailableTypes;
// namespace MultipleBotFrameworkUpgrade.Utils;
//
// /// <summary>
// /// Создание IReplyMarkup из строки. 
// /// </summary>
// /// <remarks>
// /// Пример строки IReplyMarkup:
// /// [Кнопка 1#callback_1] [Кнопка 2#callback2] [Кнопка 3] \n [Кнопка 4] [Кнопка 5] [Кнопка 6]
// /// </remarks>
// /// <typeparam name="T"></typeparam>
// public class StringMarkupBuilder<T> where T : ReplyMarkup
// {
//     private static Regex buttonRegex = new(@"\[(?<button>\S[^#\[]*)(\s*|(\#(?<callback>\w*)))\]");
//     
//     public T Build(string str)
//     {
//         str = str.Trim(' ', '\n');
//         MarkupBuilder<T> mb = new();
//         IsMarkupStringValid(str);
//         List<string> lines = str.Split('\n').ToList();
//         
//         foreach (string line in lines)
//         {
//             mb.NewRow();
//             MatchCollection mc = buttonRegex.Matches(line);
//             foreach (Match match in mc)
//             {
//                 mb.Add(match.Groups["button"].Value, match.Groups["callback"]?.Value ?? null);
//             }
//         }
//
//         return (T)mb.Build();
//     }
//
//     /// <summary>
//     /// Проверяем 
//     /// - Чтобы для каждой открывающей скобки была соответствующая закрывающая.
//     /// - Чтобы у скобок не было вложенности.
//     /// - Чтобы не было пустых строк в скобках.
//     /// </summary>
//     /// <param name="str"></param>
//     /// <returns></returns>
//     /// <exception cref="InvalidStringMarkupException"></exception>
//     private bool IsMarkupStringValid(string str)
//     {
//         // Проверяем, чтобы все скобки последовательно открывались и закрывались, без вложенности.
//         // Например [] [] [], а не [] [[]] [.
//
//         bool b = false;
//         int lastOpen = int.MinValue;
//         for(int i = 0; i < str.Length; i++)
//         {
//             char ch = str[i];
//             if(ch.Equals('[') == false && 
//                ch.Equals(']') == false &&
//                ((char.IsWhiteSpace(ch) || ch.Equals('\n')) && b == false || 
//                 (b == true))
//                ) continue;
//
//             if (ch.Equals('[') && b == false)
//             {
//                 b = true;
//                 lastOpen = i;
//             }
//             else if (ch.Equals(']') && b == true && (i - lastOpen) > 1)
//             {
//                 b = false;
//             }
//             else throw new InvalidStringMarkupException(str);
//         }
//         
//         if(b) throw new InvalidStringMarkupException(str);
//
//         return true;
//     }
// }