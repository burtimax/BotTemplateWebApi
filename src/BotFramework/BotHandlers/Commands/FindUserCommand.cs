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
/// Команда для поиска пользователей.
/// /find {string}
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserGet})]
public class FindUserCommand: BaseBotCommand
{
    internal const string Name = "/find";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public FindUserCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        int skip = 0;
        int limit = 10;
        
        string command = update.Message.Text?.Trim(' ', '.');
        string[] words = command.Split(' ', ',', '.')[1..];
        
        if (words == null || words.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Отсутствует строка поиска.\n" +
                                                        "Например [/find {string}]");
            return;
        }

        var users = await _baseBotRepository.SearchUsers(string.Join(' ', words), skip, limit);

        if (users == null || users.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Не найдены пользователи.");
            return;
        }
        
        await BotClient.SendTextMessageAsync(Chat.ChatId, $"<b>Найденные пользователи:</b>\n" + GetUsersString(users), parseMode:ParseMode.Html);
    }

    private string GetUsersString(IEnumerable<BotUser> users)
    {
        StringBuilder sb = new();
        foreach (BotUser user in users)
        {
            sb.AppendLine($"<code>{user.TelegramId}</code> {user.GetUsernameLink()} {user.TelegramFirstname} {user.TelegramLastname}");
        }

        return sb.ToString();
    }
}