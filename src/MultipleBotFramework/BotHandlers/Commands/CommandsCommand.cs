using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда для отображения всех доступных базовых Telegram команд с учетом разрешений.
/// /commands
/// </summary>
[BotCommand(Name, version: 1.0f)]
[BotHandler(command:Name, version: 1.0f)]
public class CommandsCommand: BaseBotHandler
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
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotClaimsGet}</code>");
            sb.AppendLine($"{ClaimsCommand.Name} - <i>Получить список всех разрешений бота.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotReportGet))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotReportGet}</code>");
            sb.AppendLine($"{ReportCommand.Name} <code>{{int hours}}</code> - <i>Получить отчет по боту.</i>");
            sb.AppendLine($"{SaveMessageCommand.Name} - <i>Сохранение сообщения в БД (в ответ на сообщение).</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserClaimCreate))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotUserClaimCreate}</code>");
            sb.AppendLine($"{SetClaimsCommand.Name} <code>{{@user}} {{число|строка}} {{число|строка}}</code> - <i>Добавить разрешения пользователю.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserClaimDelete))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotUserClaimDelete}</code>");
            sb.AppendLine($"{ResetClaimsCommand.Name} <code>{{@user}} {{число|строка}} {{число|строка}}</code> - <i>Удалить разрешения у пользователя.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserBlock))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotUserBlock}</code>");
            sb.AppendLine($"{BlockUserCommand.Name} <code>{{@user|user_id}} {{@user|user_id}}</code> - <i>Заблокировать пользователей.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserUnblock))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotUserUnblock}</code>");
            sb.AppendLine($"{UnblockUserCommand.Name} <code>{{@user|user_id}} {{@user|user_id}}</code> - <i>Разблокировать пользователей.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserGet))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotUserGet}</code>");
            sb.AppendLine($"{FindUserCommand.Name} <code>{{string}}</code> - <i>Искать пользователей.</i>");
            sb.AppendLine($"{MeCommand.Name} - <i>Получить информацию обо мне.</i>");
        }
        if (HasUserClaim(BotConstants.BaseBotClaims.BotUserNotificationSend))
        {
            sb.AppendLine($"<code>{BotConstants.BaseBotClaims.BotUserNotificationSend}</code>");
            sb.AppendLine($"{NotifyCommand.Name} - <i>Отправить уведомление всем пользователям (в ответ на сообщение).</i>");
            sb.AppendLine($"{NotifyTestCommand.Name} - <i>Тестовое уведомление для меня (в ответ на сообщение).</i>");
        }
        

        await BotClient.SendMessageAsync(Chat.ChatId, sb.ToString(), parseMode:ParseMode.Html);
    }

    private bool HasUserClaim(string claimName)
    {
        return UserClaims.Any(uc => uc.Name == claimName || uc.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty);
    }
}