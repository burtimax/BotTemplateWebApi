using BotFramework.Attributes;
using BotFramework.Base;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.BotHandlers.Commands.Hello;

/// <summary>
/// Приведен пример создания обработчика команд бота по типу [/command].
/// Пользователь шлет команду [/hello {ИМЯ}], а бот присылает [Привет {ИМЯ}].
/// </summary>
[BotCommand("/hello", version:2.1)]
public class ProcessBotCommandSample: BaseBotCommand
{
    public ProcessBotCommandSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        string command = update.Message.Text;
        string name = command.Replace("/hello", "").Trim(' ');
        await BotClient.SendTextMessageAsync(Chat.ChatId, $"Привет {name}");
    }
}