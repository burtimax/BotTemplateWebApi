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
public class NotifyAllCommand : NotifyTestCommand
{
    public const string Name = "/notify";
    
    private readonly IBroadcastTaskService _broadcastTaskService;
    
    public NotifyAllCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _broadcastTaskService = serviceProvider.GetRequiredService<IBroadcastTaskService>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message?.ReplyToMessage == null)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Ответь на какое-нибудь сообщение");
            return;
        }

        string text = update.Message.Text!.Replace(Name, "").Trim(' ');
        bool parsed = TryParseInlineKeyboard(text, out string error, out var kb);

        if (error != null)
        {
            await Answer(error);
            return;
        }
        
        var users = await BotDbContext.Chats
            .Include(c => c.BotUser)
            .Where(c => c.BotUser != null && c.BotUser.IsBlocked == false && c.BotUser.Status != BotUserStatus.Banned
            && c.Type == ChatTypes.Private && c.BotId == BotId)
            .Select(c => c.ChatId)
            .ToListAsync();
        
        await BotClient.CopyMessageAsync(Chat.ChatId, Chat.ChatId,
            update.Message.ReplyToMessage.MessageId, replyMarkup: kb?.Build());
        
        await _broadcastTaskService.NewBroadcastTask(BotClient.Options.BotToken, BotId, message: update.Message.ReplyToMessage,
            users, kb?.Build());
        
    }
}