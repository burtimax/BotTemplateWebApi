﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Models;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Utils;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class NotifyCommand : BaseBotCommand
{
    public const string Name = "/notify";

    private readonly string _dbConnection;
    
    public NotifyCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _dbConnection = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value.DbConnection;
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message?.ReplyToMessage == null)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Ответь на какое-нибудь сообщение");
            return;
        }

        // Не будем ждать окончания работы.
        Task.Factory.StartNew(async () =>
        {
            // Создаем DbContext чтобы задача могла в фоне выполняться.
            var ob = new DbContextOptionsBuilder<BotDbContext>();
            ob.UseNpgsql(_dbConnection);
            
            using (BotDbContext db = new BotDbContext(ob.Options))
            {
                await BotHelper.ExecuteForAllUsers(db, BotId,async tuple =>
                {
                    try
                    {
                        await BotClient.CopyMessageAsync(tuple.chat.ChatId, Chat.ChatId,
                            update.Message.ReplyToMessage.MessageId);
                        await Task.Delay(100); // Чтобы не выйти за лимиты бота и его не заблокировали.
                    }
                    catch (BotRequestException e) when (e.ErrorCode == 403)
                    {
                        Debug.WriteLine(e.Message, "ERROR");
                    }
                });
            }
        }, TaskCreationOptions.LongRunning);
    }
}