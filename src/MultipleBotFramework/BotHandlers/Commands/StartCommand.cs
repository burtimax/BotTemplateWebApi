using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Обязательная команда бота (по умолчанию)
/// </summary>
[BotCommand(Name, version: 0.1f)]
public class StartCommand : BaseBotCommand
{
    internal const string Name = "/start";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;

    public StartCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        await BotClient.SendTextMessageAsync(Chat.ChatId, "Default framework handler", parseMode:ParseMode.Html);
    }
}