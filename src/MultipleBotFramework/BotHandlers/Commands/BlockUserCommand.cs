using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда для блокировки пользователей.
/// /block {@user|user_id} {@user|user_id} ... 
/// </summary>
[BotHandler(command:Name, version:1f, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserBlock})]
[BotCommand(command:Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserBlock})]
public class BlockChatCommand: BaseBotHandler
{
    internal const string Name = "/block";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public BlockChatCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        HandlerDescription = BlockCommandDescription;
        _botConfiguration = serviceProvider.GetRequiredService<BotConfiguration>();
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string command = update.Message.Text?.Trim(' ', '.');
        string[] chats = command.Split(' ', ',', '.')[1..];

        if (chats == null || chats.Any() == false)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, BlockCommandTutorial);
            return;
        }

        List<BotChatEntity> chatsToBlock = new ();

        foreach (string chatIdStr in chats)
        {
            long chatId = long.Parse(chatIdStr);
            BotChatEntity? chat = await _baseBotRepository.GetChatById(BotId, chatId);
            IEnumerable<BotClaimEntity> userClaims = await _baseBotRepository.GetUserClaims(BotId, chat.Id);
            
            // Админов нельзя блокировать.
            if (userClaims!= null && 
                userClaims.Any(uc => uc.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty))
            {
                continue;
            }

            if (chat == null)
            {
                await BotClient.SendMessageAsync(Chat.ChatId, NotFoundChat.F(chatId) + "\n" + BlockCommandTutorial);
                return;
            }
            
            chatsToBlock.Add(chat);
        }
        
        await _baseBotRepository.BlockChats(BotId, chatsToBlock.Select(u => u.Id).ToArray());

        await BotClient.SendMessageAsync(Chat.ChatId, "Чаты заблокированы.");
    }
    
}