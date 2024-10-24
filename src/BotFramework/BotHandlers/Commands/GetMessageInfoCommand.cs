using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class GetMessageInfoCommand : BaseBotCommand
{
    public const string Name = "/msg";

    private readonly string _dbConnection;
    
    public GetMessageInfoCommand(IServiceProvider serviceProvider) : base(serviceProvider)
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

        Message replyMessage = update.Message.ReplyToMessage!;
        
        StringBuilder sb = new();
        sb.AppendLine("<b>Данные по сообщению</b>")
          .AppendLine($"<b>ChatId</b> - <code>{replyMessage.Chat.Id}</code>")
          .AppendLine($"<b>MessageId</b> - <code>{replyMessage.MessageId}</code>");

        await BotClient.SendTextMessageAsync(Chat.ChatId, sb.ToString(), parseMode: ParseMode.Html);
    }
}