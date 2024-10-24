using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MultipleBotFramework_Obsolete.BotHandlers.Commands;

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

        List<BotUserEntity> usersToBlock = new ();

        foreach (string userIdentity in users)
        {
            BotUserEntity? user = await _baseBotRepository.GetUserByIdentity(BotId, userIdentity);
            IEnumerable<BotClaimEntity> userClaims = await _baseBotRepository.GetUserClaims(BotId, user.Id);
            
            // Админов нельзя блокировать.
            if (userClaims!= null && 
                userClaims.Any(uc => uc.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty))
            {
                continue;
            }

            if (user == null)
            {
                await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userIdentity}].\n" + 
                                                                  "Необходимо указать параметры команды.\n" +
                                                                  "Например [/block {@user|user_id} {@user|user_id} ...]");
                return;
            }
            
            usersToBlock.Add(user);
        }

        
        
        await _baseBotRepository.BlockUsers(BotId, usersToBlock.Select(u => u.Id).ToArray());

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Пользователи заблокированы.");
    }
    
}