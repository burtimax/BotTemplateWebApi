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
/// Команда для отображения информации по пользователю.
/// /user {string}
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new[] { BotConstants.BaseBotClaims.BotUserGet })]
public class UserCommand : BaseBotCommand
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
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Введите идентификатор пользователя.\n" +
                                                              "Например [/user {string}]");
            return;
        }

        string userIdentity = words[1];

        BotUser? user = await _baseBotRepository.GetUserByIdentity(userIdentity);

        if (user == null)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId,
                $"Не найден пользователь в идентификатором {userIdentity}.");
            return;
        }

        BotChat? userChat = await _baseBotRepository.GetChat(Chat.ChatId, user.Id);
        IEnumerable<BotClaim>? claims = await _baseBotRepository.GetUserClaims(user.Id);
        string userData = GetUserDataString(user!, userChat, claims);

        await BotClient.SendTextMessageAsync(Chat.ChatId, userData, ParseMode.Html);
    }

    /// <summary>
    /// Формирует информацию о пользователе.
    /// </summary>
    public static string GetUserDataString(BotUser user, BotChat chat, IEnumerable<BotClaim>? claims)
    {
        string blocked = user.IsBlocked ? "заблокирован" : "активен";

        StringBuilder sb = new();
        sb.AppendLine($"<b>Пользователь</b>: <code>{user.TelegramId}</code>");
        sb.AppendLine($"<b>Имя</b>: {user.TelegramFirstname} {user.TelegramLastname}");
        sb.AppendLine($"<b>Ник</b>: <code>{user.GetUsernameLink()}</code>");
        sb.AppendLine($"<b>Язык</b>: {user.LanguageCode}");
        sb.AppendLine($"<b>Телефон</b>: {user.Phone}");
        sb.AppendLine($"<b>Статус</b>: {blocked}");
        sb.AppendLine($"<b>Текущее состояние чата</b>: {string.Join('/', chat.States)}");
        sb.AppendLine(ClaimsCommand.GenerateClaimsListString(claims, "Разрешения пользователя"));
        return sb.ToString();
    }
}