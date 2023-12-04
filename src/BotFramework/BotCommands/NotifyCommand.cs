using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db;
using BotFramework.Extensions;
using BotFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace BotFramework.BotCommands;

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
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Ответь на какое-нибудь сообщение");
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
                await BotHelper.ExecuteForAllUsers(db, async tuple =>
                {
                    try
                    {
                        await BotClient.CopyMessageAsync(tuple.chat.ChatId, Chat.ChatId,
                            update.Message.ReplyToMessage.MessageId);
                    }
                    catch (ApiRequestException e) when (e.ErrorCode == 403)
                    {
                        Debug.WriteLine(e.Message, "ERROR");
                    }
                });
            }
        }, TaskCreationOptions.LongRunning);
    }
}