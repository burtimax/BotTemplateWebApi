using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Constants;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;
using MultipleBotFrameworkUpgrade.Models;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Repository;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда для отображения всех возможных разрешений бота.
/// /claims
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotClaimsGet})]
[BotHandler(command:Name, version: 1.0f, requiredUserClaims: new []{BotConstants.BaseBotClaims.BotClaimsGet})]
public class ClaimsCommand: BaseBotHandler
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
            await BotClient.SendMessageAsync(Chat.ChatId, "У бота нет зарегистрированных разрешений!", parseMode:ParseMode.Html);
            return;
        }

        await BotClient.SendMessageAsync(Chat.ChatId, GenerateClaimsListString(claims, "Разрешения бота"), parseMode:ParseMode.Html);
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