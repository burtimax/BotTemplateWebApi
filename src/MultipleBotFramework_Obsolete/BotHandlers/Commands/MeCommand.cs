using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework_Obsolete.Attributes;
using MultipleBotFramework_Obsolete.Base;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Options;
using MultipleBotFramework_Obsolete.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework_Obsolete.BotHandlers.Commands;

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
        BotChatEntity? userChat = await _baseBotRepository.GetChat(BotId, Chat.ChatId, User.Id);
        IEnumerable<BotClaimEntity>? claims = await _baseBotRepository.GetUserClaims(BotId, User.Id);
        string userData = UserCommand.GetUserDataString(User!, userChat, claims);

        await BotClient.SendTextMessageAsync(Chat.ChatId, userData, parseMode:ParseMode.Html);
    }
}