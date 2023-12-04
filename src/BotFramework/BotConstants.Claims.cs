using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BotFramework
{
    public partial class BotConstants
    {
        /// <summary>
        /// Базовые разрешения пользователя.
        /// </summary>
        public class BaseBotClaims
        {
            /// <summary>
            /// Все разрешения доступны, можешь делать что хочешь.
            /// </summary>
            internal const string IAmBruceAlmighty = "я.есть.брюс.всемогущий"; // Internal потому что внешнее приложение не должно раздавать доступ админа, его можно получить только авторизовавшись, как админ.
            internal const string IAmBruceAlmightyDescription = "Все разрешения доступны, можешь делать что хочешь.";

            /// <summary>
            /// Добавление разрешений пользователям.
            /// </summary>
            public const string BotUserClaimCreate = "base.bot.user.claim.create";
            public const string BotUserClaimCreateDescription = "Добавление разрешений пользователям.";

            /// <summary>
            /// Снятие разрешений у пользователей.
            /// </summary>
            public const string BotUserClaimDelete = "base.bot.user.claim.delete";
            public const string BotUserClaimDeleteDescription = "Снятие разрешений у пользователей.";
            
            /// <summary>
            /// Получение данных по разрешениям пользователей.
            /// </summary>
            public const string BotUserClaimGet = "base.bot.user.claim.get";
            public const string BotUserClaimGetDescription = "Получение данных по разрешениям пользователей.";

            /// <summary>
            /// Получение данных по разрешениям пользователей.
            /// </summary>
            public const string BotUserNotificationSend = "base.bot.user.notification.send";
            public const string BotUserNotificationSendDescription = "Рассылка уведомлений для пользователей.";
            
            /// <summary>
            /// Получение информации по пользователям.
            /// </summary>
            public const string BotUserGet = "base.bot.user.get";
            public const string BotUserGetDescription = "Получение информации по пользователям.";

            /// <summary>
            /// Блокировка пользователей бота.
            /// </summary>
            public const string BotUserBlock = "base.bot.user.block";
            public const string BotUserBlockDescription = "Блокировка пользователей бота.";

            /// <summary>
            /// Разблокировка пользователей бота.
            /// </summary>
            public const string BotUserUnblock = "base.bot.user.unblock";
            public const string BotUserUnblockDescription = "Разблокировка пользователей бота.";

            /// <summary>
            /// Получение статистики/отчета активности бота.
            /// </summary>
            public const string BotReportGet = "base.bot.report.get";
            public const string BotReportGetDescription = "Получение статистики/отчета активности бота.";

            /// <summary>
            /// Получение списка доступных команд управления.
            /// </summary>
            public const string BotCommandGet = "base.bot.command.get";
            public const string BotCommandGetDescription = "Получение списка доступных команд управления.";
            
            /// <summary>
            /// Получение списка всех разрешений бота.
            /// </summary>
            /// <remarks>
            /// Удобно использовать админу, как подсказка.
            /// </remarks>
            public const string BotClaimsGet = "base.bot.claims.get";
            public const string BotClaimsGetDescription = "Получение списка всех разрешений бота.";
            
            /// <summary>
            /// Получение отчетов по ошибкам бота.
            /// </summary>
            public const string BotExceptionsGet = "base.bot.exception.get";
            public const string BotExceptionsGetDescription = "Получение отчетов по ошибкам бота.";
            
            /// <summary>
            /// Получить все базовые клэймы бота.
            /// </summary>
            /// <returns>Возвращает все константные значения клэймов в виде списка <see cref="ClaimValue"/>.</returns>
            internal static IEnumerable<ClaimValue> GetBaseBotClaims()
            {
                BaseBotClaims instance = new(); 
                List<FieldInfo> claimNameFields = typeof(BaseBotClaims).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(string))
                    .Where(fi => fi.Name.EndsWith("Description") == false)
                    .Append(typeof(BaseBotClaims).GetField(nameof(IAmBruceAlmighty), BindingFlags.NonPublic | BindingFlags.Static))
                    .ToList();

                IEnumerable<ClaimValue> claimsRes = new List<ClaimValue>();

                foreach (var fieldClaimName in claimNameFields)
                {
                    FieldInfo? prop = typeof(BaseBotClaims).GetField($"{fieldClaimName.Name}Description", BindingFlags.Public | BindingFlags.Static) ??
                                      typeof(BaseBotClaims).GetField($"{fieldClaimName.Name}Description", BindingFlags.NonPublic | BindingFlags.Static);
                    claimsRes = claimsRes.Append(new ClaimValue(fieldClaimName.GetValue(instance) as string,
                        prop.GetValue(instance) as string));
                }
                
                return claimsRes;
            }
        }
    }
}