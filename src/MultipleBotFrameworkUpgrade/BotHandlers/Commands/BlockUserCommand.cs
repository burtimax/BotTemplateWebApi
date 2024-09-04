using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
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
/// Команда для блокировки пользователей.
/// /block {@user|user_id} {@user|user_id} ... 
/// </summary>
[BotHandler(command:Name, version:1f, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserBlock})]
[BotCommand(command:Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserBlock})]
public class BlockUserCommand: BaseBotHandler
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
            await BotClient.SendMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
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
                await BotClient.SendMessageAsync(Chat.ChatId, $"Не найден пользователь [{userIdentity}].\n" + 
                                                                  "Необходимо указать параметры команды.\n" +
                                                                  "Например [/block {@user|user_id} {@user|user_id} ...]");
                return;
            }
            
            usersToBlock.Add(user);
        }

        
        
        await _baseBotRepository.BlockUsers(BotId, usersToBlock.Select(u => u.Id).ToArray());

        await BotClient.SendMessageAsync(Chat.ChatId, "Пользователи заблокированы.");
    }
    
}