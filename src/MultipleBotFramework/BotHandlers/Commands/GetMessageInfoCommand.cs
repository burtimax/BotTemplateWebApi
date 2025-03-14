﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Options;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
[BotHandler(command:Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class GetMessageInfoCommand : BaseBotHandler
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
            await BotClient.SendMessageAsync(Chat.ChatId, "Ответь на какое-нибудь сообщение");
            return;
        }

        Message replyMessage = update.Message.ReplyToMessage!;
        
        StringBuilder sb = new();
        sb.AppendLine("<b>Данные по сообщению</b>")
          .AppendLine($"<b>ChatId</b> - <code>{replyMessage.Chat.Id}</code>")
          .AppendLine($"<b>MessageId</b> - <code>{replyMessage.MessageId}</code>");

        await BotClient.SendMessageAsync(Chat.ChatId, sb.ToString(), parseMode: ParseMode.Html);
    }
}