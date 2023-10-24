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
/// Команда для блокировки пользователей.
/// /block {@user|user_id} {@user|user_id} ... 
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserBlock})]
public class BlockUserCommand: BaseBotCommand
{
    internal const string Name = "/block";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public BlockUserCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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
                                                        "Например [/block {@user|user_id} {@user|user_id} ...]");
            return;
        }

        List<BotUser> usersToBlock = new ();

        foreach (string userIdentity in users)
        {
            BotUser? user = await _baseBotRepository.GetUserByIdentity(userIdentity);

            if (user == null)
            {
                await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userIdentity}].\n" + 
                                                                  "Необходимо указать параметры команды.\n" +
                                                                  "Например [/block {@user|user_id} {@user|user_id} ...]");
                return;
            }
            
            usersToBlock.Add(user);
        }

        await _baseBotRepository.BlockUsers(usersToBlock.Select(u => u.Id).ToArray());

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Пользователи заблокированы.");
    }
    
}