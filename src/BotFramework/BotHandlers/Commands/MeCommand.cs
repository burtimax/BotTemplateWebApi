using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db.Entity;
using BotFramework.Exceptions;
using BotFramework.Options;
using BotFramework.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands;

/// <summary>
/// Команда для отображения информации по мне.
/// /me
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new[] { BotConstants.BaseBotClaims.BotUserGet })]
public class MeCommand : BaseBotCommand
{
    internal const string Name = "/me";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;

    public MeCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        BotChat? userChat = await _baseBotRepository.GetChat(Chat.ChatId, User.Id);
        IEnumerable<BotClaim>? claims = await _baseBotRepository.GetUserClaims(User.Id);
        string userData = UserCommand.GetUserDataString(User!, userChat, claims);

        await BotClient.SendTextMessageAsync(Chat.ChatId, userData, parseMode:ParseMode.Html);
    }
}