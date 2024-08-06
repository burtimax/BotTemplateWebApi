using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.BotHandlers.States.SaveMessage;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Enums;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class SaveMessageCommand : BaseBotCommand
{
    public const string Name = "/save_msg";

    private readonly BotDbContext _db;
    
    public SaveMessageCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _db = serviceProvider.GetRequiredService<BotDbContext>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (Chat.States.CurrentState != SaveMessageState.Name)
        {
            Chat.States.Set(SaveMessageState.Name, ChatStateSetterType.SetNext);
            await _db.SaveChangesAsync();
        }
        
        await BotClient.SendMessageAsync(Chat.ChatId, "Отправьте сообщение, которое будет сохранено");
    }
}