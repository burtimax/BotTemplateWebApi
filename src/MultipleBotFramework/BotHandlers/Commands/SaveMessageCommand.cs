using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.BotHandlers.States.SaveMessage;
using MultipleBotFramework.Db;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Enums;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
[BotHandler(command: Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class SaveMessageCommand : BaseBotHandler
{
    public const string Name = "/save_msg";

    private readonly BotDbContext _db;
    
    public SaveMessageCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _db = serviceProvider.GetRequiredService<BotDbContext>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (Chat.States.CurrentState != SaveMessageHandler.Name)
        {
            Chat.States.Set(SaveMessageHandler.Name, ChatStateSetterType.SetNext);
            await _db.SaveChangesAsync();
        }
        
        await BotClient.SendMessageAsync(Chat.ChatId, "Отправьте сообщение, которое будет сохранено");
    }
}