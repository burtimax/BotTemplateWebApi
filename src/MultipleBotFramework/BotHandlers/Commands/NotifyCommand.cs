using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Options;
using MultipleBotFramework.Quartz.Jobs.Notification;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
[BotHandler(command:Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class NotifyAllCommand : BaseBotHandler
{
    public const string Name = "/notify";

    private readonly string _dbConnection;
    private readonly IBotNotificationTasksService _notificationTasksService;
    
    public NotifyAllCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _dbConnection = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value.DbConnection;
        _notificationTasksService = serviceProvider.GetRequiredService<IBotNotificationTasksService>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message?.ReplyToMessage == null)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Ответь на какое-нибудь сообщение");
            return;
        }

        var users = await BotDbContext.Users
            .Where(u => u.IsBlocked == false && u.Status != BotUserStatus.Banned)
            .Select(u => u.TelegramId)
            .ToListAsync();

        BotNotificationTask botTask = new()
        {
            BotId = BotId,
            ChatIds = users,
            Action = async (client, chatId) => await BotClient.CopyMessageAsync(chatId, Chat.ChatId,
                update.Message.ReplyToMessage.MessageId)
        };

        await _notificationTasksService.AddNotificationTask(botTask);

    }
}