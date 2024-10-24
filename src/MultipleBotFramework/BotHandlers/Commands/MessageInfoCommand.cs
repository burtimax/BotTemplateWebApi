using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.BotHandlers.States.SaveMessage;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotHandler(command: Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotReportGet })]
public class MessageInfoCommand : BaseBotHandler
{
    public const string Name = "/info_msg";

    private readonly BotDbContext _db;
    
    public MessageInfoCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _db = serviceProvider.GetRequiredService<BotDbContext>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        List<string> messages = new();
        string json = update.ToJson();
        json = Regex.Unescape(json);
        int l = json.Length;

        for (int i = 0; i <= l / BotConstants.Constraints.MaxMessageLength; i++)
        {
            int start = i * BotConstants.Constraints.MaxMessageLength;
            string msg = json.Substring(start, Math.Min(l - start, BotConstants.Constraints.MaxMessageLength));
            await BotClient.SendMessageAsync(Chat.ChatId, $"<code>{msg}</code>", parseMode: ParseMode.Html);
        }
    }
}