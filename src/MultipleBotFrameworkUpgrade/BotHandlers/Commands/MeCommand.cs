using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Constants;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;
using MultipleBotFrameworkUpgrade.Models;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Repository;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда для отображения информации по мне.
/// /me
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new[] { BotConstants.BaseBotClaims.BotUserGet })]
[BotHandler(command:Name, version: 1.0f, requiredUserClaims: new[] { BotConstants.BaseBotClaims.BotUserGet })]
public class MeCommand : BaseBotHandler
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
        //BotChatEntity? userChat = await _baseBotRepository.GetChat(BotId, Chat.ChatId, User.Id);
        BotChatEntity? userChat = await _baseBotRepository.GetChatById(BotId, Chat.ChatId);
        IEnumerable<BotClaimEntity>? claims = await _baseBotRepository.GetUserClaims(BotId, User.Id);
        string userData = UserCommand.GetUserDataString(User!, userChat, claims);

        await BotClient.SendMessageAsync(Chat.ChatId, userData, parseMode:ParseMode.Html);
    }
}