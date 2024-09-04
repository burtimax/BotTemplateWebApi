using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Constants;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Repository;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда для отображения информации по пользователю.
/// /user {string}
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new[] { BotConstants.BaseBotClaims.BotUserGet })]
[BotHandler(command: Name, version: 1.0f, requiredUserClaims: new[] { BotConstants.BaseBotClaims.BotUserGet })]
public class UserCommand : BaseBotHandler
{
    internal const string Name = "/user";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;

    public UserCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string[] words = update.Message.Text.Split(' ');
        if (words.Length < 2)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Введите идентификатор пользователя.\n" +
                                                              "Например [/user {string}]");
            return;
        }

        string userIdentity = words[1];

        BotUserEntity? user = await _baseBotRepository.GetUserByIdentity(BotId, userIdentity);

        if (user == null)
        {
            await BotClient.SendMessageAsync(Chat.ChatId,
                $"Не найден пользователь в идентификатором {userIdentity}.");
            return;
        }

        //BotChatEntity? userChat = await _baseBotRepository.GetChat(BotId, Chat.ChatId, user.Id);
        BotChatEntity? userChat = await _baseBotRepository.GetChatById(BotId, Chat.ChatId);
        IEnumerable<BotClaimEntity>? claims = await _baseBotRepository.GetUserClaims(BotId, user.Id);
        string userData = GetUserDataString(user!, userChat, claims);

        await BotClient.SendMessageAsync(Chat.ChatId, userData, parseMode:ParseMode.Html);
    }

    /// <summary>
    /// Формирует информацию о пользователе.
    /// </summary>
    public static string GetUserDataString(BotUserEntity userEntity, BotChatEntity chatEntity, IEnumerable<BotClaimEntity>? claims)
    {
        string blocked = userEntity.IsBlocked ? "заблокирован" : "активен";

        StringBuilder sb = new();
        sb.AppendLine($"<b>Пользователь</b>: <code>{userEntity.TelegramId}</code>");
        sb.AppendLine($"<b>Имя</b>: {userEntity.TelegramFirstname} {userEntity.TelegramLastname}");
        sb.AppendLine($"<b>Ник</b>: <code>{userEntity.GetUsernameLink()}</code>");
        sb.AppendLine($"<b>Язык</b>: {userEntity.LanguageCode}");
        sb.AppendLine($"<b>Телефон</b>: {userEntity.Phone}");
        sb.AppendLine($"<b>Статус</b>: {blocked}");
        sb.AppendLine($"<b>Текущее состояние чата</b>: {string.Join('/', chatEntity.States)}");
        sb.AppendLine(ClaimsCommand.GenerateClaimsListString(claims, "Разрешения пользователя"));
        return sb.ToString();
    }
}