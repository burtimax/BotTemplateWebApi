using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.BotCommands.Admin;
using BotFramework.Db.Entity;
using BotFramework.Options;
using BotFramework.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands;

/// <summary>
/// Команда для отображения всех доступных базовых Telegram команд с учетом разрешений.
/// /commands
/// </summary>
[BotCommand(Name, version: 1.0f)]
public class CommandsCommand: BaseBotCommand
{
    internal const string Name = "/commands";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public CommandsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        StringBuilder sb = new();
        sb.AppendLine("<b>Команды управления:</b>");

        sb.AppendLine($"{CommandsCommand.Name} - <i>Получить список доступных команд управления.</i>");
      
        if (HasUserClaim(BotConstants.BaseBotClaims.BotClaimsGet))
        {
            sb.AppendLine($"{ClaimsCommand.Name} - <i>Получить список всех разрешений бота.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotReportGet))
        {
            sb.AppendLine($"{ReportCommand.Name} - <i>Получить отчет по боту.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserClaimCreate))
        {
            sb.AppendLine($"{SetClaimsCommand.Name} - <i>Добавить разрешения пользователю.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserClaimDelete))
        {
            sb.AppendLine($"{ResetClaimsCommand.Name} - <i>Удалить разрешения у пользователя.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserBlock))
        {
            sb.AppendLine($"{BlockUserCommand.Name} - <i>Заблокировать пользователей.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserUnblock))
        {
            sb.AppendLine($"{UnblockUserCommand.Name} - <i>Разблокировать пользователей.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserGet))
        {
            sb.AppendLine($"{FindUserCommand.Name} - <i>Искать пользователей.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserGet))
        {
            sb.AppendLine($"{MeCommand.Name} - <i>Получить информацию обо мне.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserNotificationSend))
        {
            sb.AppendLine($"{NotifyCommand.Name} - <i>Отправить уведомление всем пользователям.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserNotificationSend))
        {
            sb.AppendLine($"{NotifyTestCommand.Name} - <i>Тестовое уведомление для меня.</i>");
        }

        await BotClient.SendTextMessageAsync(Chat.ChatId, sb.ToString(), parseMode:ParseMode.Html);
    }

    private bool HasUserClaim(string claimName)
    {
        return UserClaims.Any(uc => uc.Name == claimName || uc.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty);
    }
}