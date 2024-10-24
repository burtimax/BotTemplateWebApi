using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.BotHandlers.States.SaveMessage;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Enums;
using BotFramework.Extensions;
using BotFramework.Options;
using BotFramework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands;

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
        
        await BotClient.SendTextMessageAsync(Chat.ChatId, "Отправьте сообщение, которое будет сохранено");
    }
}