using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework_Obsolete.Attributes;
using MultipleBotFramework_Obsolete.Base;
using MultipleBotFramework_Obsolete.Db;
using MultipleBotFramework_Obsolete.Options;
using MultipleBotFramework_Obsolete.Utils;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace MultipleBotFramework_Obsolete.BotHandlers.Commands;

/// <summary>
/// Команда уведомления (тест для проверки).
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class NotifyTestCommand : BaseBotCommand
{
    public const string Name = "/testnotify";

    private readonly string _dbConnection;
    
    public NotifyTestCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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
                await BotHelper.ExecuteFor(db, BotId, new List<long>(){ User.TelegramId }, async tuple =>
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