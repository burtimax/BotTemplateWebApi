using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db.Entity;
using BotFramework.Exceptions;
using BotFramework.Options;
using BotFramework.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands;

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
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/unblock {@user|user_id} {@user|user_id} ...]");
            return;
        }

        List<BotUser> usersToUnblock = new ();

        foreach (string userIdentity in users)
        {
            BotUser? user = await _baseBotRepository.GetUserByIdentity(userIdentity);

            if (user == null)
            {
                await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userIdentity}].\n" + 
                                                                  "Необходимо указать параметры команды.\n" +
                                                                  "Например [/unblock {@user|user_id} {@user|user_id} ...]");
                return;
            }
            
            usersToUnblock.Add(user);
        }

        await _baseBotRepository.UnblockUsers(usersToUnblock.Select(u => u.Id).ToArray());

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Пользователи разблокированы.");
    }
    
}