using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Repository;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда для разблокировки пользователей.
/// /unblock {@user|user_id} {@user|user_id} ... 
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserUnblock})]
public class UnblockUserCommand: BaseBotCommand
{
    internal const string Name = "/unblock";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public UnblockUserCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string command = update.Message.Text?.Trim(' ', '.');
        string[] users = command.Split(' ', ',', '.')[1..];

        if (users == null || users.Any() == false)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/unblock {@user|user_id} {@user|user_id} ...]");
            return;
        }

        List<BotUserEntity> usersToUnblock = new ();

        foreach (string userIdentity in users)
        {
            BotUserEntity? user = await _baseBotRepository.GetUserByIdentity(BotId, userIdentity);

            if (user == null)
            {
                await BotClient.SendMessageAsync(Chat.ChatId, $"Не найден пользователь [{userIdentity}].\n" + 
                                                                  "Необходимо указать параметры команды.\n" +
                                                                  "Например [/unblock {@user|user_id} {@user|user_id} ...]");
                return;
            }
            
            usersToUnblock.Add(user);
        }

        await _baseBotRepository.UnblockUsers(BotId, usersToUnblock.Select(u => u.Id).ToArray());

        await BotClient.SendMessageAsync(Chat.ChatId, "Пользователи разблокированы.");
    }
    
}