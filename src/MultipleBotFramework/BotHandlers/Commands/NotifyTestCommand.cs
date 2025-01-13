using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
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
/// Команда уведомления (тест для проверки).
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
[BotHandler(command: Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class NotifyTestCommand : BaseBotHandler
{
    public const string Name = "/testnotify";

    private readonly string _dbConnection;
    private readonly IBotNotificationTasksService _notificationTasksService;
    
    public NotifyTestCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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

        // Не будем ждать окончания работы.

        BotNotificationTask botTask = new();
        botTask.BotId = BotId;
        botTask.ChatIds.Add(update.Message.Chat.Id);
        botTask.Action = async (botClient, chatId) => await BotClient.CopyMessageAsync(chatId, Chat.ChatId,
            update.Message.ReplyToMessage.MessageId);
        await _notificationTasksService.AddNotificationTask(botTask);
    }
}