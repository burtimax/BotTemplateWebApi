using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework_Obsolete.Attributes;
using MultipleBotFramework_Obsolete.Base;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Options;
using MultipleBotFramework_Obsolete.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework_Obsolete.BotHandlers.Commands;

/// <summary>
/// Команда для отображения всех возможных разрешений бота.
/// /claims
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotClaimsGet})]
public class ClaimsCommand: BaseBotCommand
{
    internal const string Name = "/claims";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public ClaimsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        IEnumerable<BotClaimEntity> claims = await _baseBotRepository.GetAllClaims(true);

        if (claims == null || claims.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "У бота нет зарегистрированных разрешений!", parseMode:ParseMode.Html);
            return;
        }

        await BotClient.SendTextMessageAsync(Chat.ChatId, GenerateClaimsListString(claims, "Разрешения бота"), parseMode:ParseMode.Html);
    }

    /// <summary>
    /// Для списка разрешений сгенерировать строку для Telegram.
    /// </summary>
    /// <param name="claims">Список разрешений</param>
    /// <returns></returns>
    public static string GenerateClaimsListString(IEnumerable<BotClaimEntity>? claims, string title = null)
    {
        if (claims == null) return "<b>Нет разрешений</b>";
        
        StringBuilder sb = new();

        if (string.IsNullOrEmpty(title) == false)
        {
            sb.AppendLine($"<b>{title}:</b>");
        }

        foreach (BotClaimEntity claim in claims)
        {
            sb.AppendLine($"<code>{claim.Id}</code> <b>-</b> <code>{claim.Name}</code> - <i>{claim.Description}</i>");
        }

        return sb.ToString();
    }
    
}