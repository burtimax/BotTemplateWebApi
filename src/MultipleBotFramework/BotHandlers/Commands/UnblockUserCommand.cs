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
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда для разблокировки пользователей.
/// /unblock {@user|user_id} {@user|user_id} ... 
/// </summary>
[BotHandler(command: Name, version: 1.0f,requiredUserClaims: new []{BotConstants.BaseBotClaims.BotUserUnblock})]
public class UnblockChatCommand: BaseBotHandler
{
    internal const string Name = "/unblock";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public UnblockChatCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<BotConfiguration>();
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string command = update.Message.Text?.Trim(' ', '.');
        string[] chats = command.Split(' ', ',', '.')[1..];

        if (chats == null || chats.Any() == false)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/unblock {chat_id} {chat_id} ...]");
            return;
        }

        List<BotChatEntity> usersToUnblock = new ();

        foreach (string chatIdStr in chats)
        {
            long chatId = long.Parse(chatIdStr);
            BotChatEntity? chat = await _baseBotRepository.GetChatById(BotId, chatId);

            if (chat == null)
            {
                await BotClient.SendMessageAsync(Chat.ChatId, $"Не найден пользователь [{chatId}].\n" + 
                                                                  "Необходимо указать параметры команды.\n" +
                                                                  "Например [/unblock {chat_id} {chat_id} ...]");
                return;
            }
            
            usersToUnblock.Add(chat);
        }

        await _baseBotRepository.UnblockChats(BotId, usersToUnblock.Select(u => u.Id).ToArray());

        await BotClient.SendMessageAsync(Chat.ChatId, "Чаты разблокированы.");
    }
    
}