using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда для поиска пользователей.
/// /find {string}
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserGet})]
[BotHandler(command:Name, version: 1.0f, requiredUserClaims: new []{BotConstants.BaseBotClaims.BotUserGet})]
public class FindUserCommand: BaseBotHandler
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
            await BotClient.SendMessageAsync(Chat.ChatId, "Отсутствует строка поиска.\n" +
                                                        "Например [/find {string}]");
            return;
        }

        var users = await _baseBotRepository.SearchUsers(BotId, string.Join(' ', words), skip, limit);

        if (users == null || users.Any() == false)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Не найдены пользователи.");
            return;
        }
        
        await BotClient.SendMessageAsync(Chat.ChatId, $"<b>Найденные пользователи:</b>\n" + GetUsersString(users), parseMode:ParseMode.Html);
    }

    private string GetUsersString(IEnumerable<BotUserEntity> users)
    {
        StringBuilder sb = new();
        foreach (BotUserEntity user in users)
        {
            sb.AppendLine($"<code>{user.TelegramId}</code> {user.GetUsernameLink()} {user.TelegramFirstname} {user.TelegramLastname}");
        }

        return sb.ToString();
    }
}