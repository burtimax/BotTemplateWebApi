using System;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db;
using BotFramework.Options;
using BotFramework.Other.ReportGenerator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands.Admin;

/// <summary>
/// Команда для регистрации админа в системе.
/// Админов может быть несколько.
/// Пример команды: [/auth {ПАРОЛЬ БОТА}]
/// </summary>
[BotCommand(ADMININFO)]
public class InfoAdminCommand: BaseBotCommand
{
    internal const string ADMININFO = "/admininfo";

    private readonly BotConfiguration _botConfiguration;
    
    public InfoAdminCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (await AdminCommandHelper.HasUserAccessToCommand(User, BotClient, Chat.ChatId, _botConfiguration) ==
            false) return;

        string info = $"<b>Команды администратора</b>:\n" +
                      $"{InfoAdminCommand.ADMININFO} - справка для админа.";
        
        await BotClient.SendTextMessageAsync(Chat.ChatId, info, ParseMode.Html);
    }
}