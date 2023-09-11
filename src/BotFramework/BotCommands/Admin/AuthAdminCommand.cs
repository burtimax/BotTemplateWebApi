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

namespace BotFramework.BotCommands.Admin;

/// <summary>
/// Команда для регистрации админа в системе.
/// Админов может быть несколько.
/// Пример команды: [/auth {ПАРОЛЬ БОТА}]
/// </summary>
[BotCommand(AUTH, BotConstants.UserRoles.Moderator)]
public class AuthAdminCommand: BaseBotCommand
{
    internal const string AUTH = "/auth";

    private readonly BotConfiguration _botConfiguration;
    
    public AuthAdminCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
    }

    public override async Task HandleBotRequest(Update update)
    {
        string[] words = update.Message.Text.Split(' ');
        if (words.Length < 2)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId,
                $"Команда не правильна, введите пароль админа.\n" +
                    $"Например: [{AUTH} ПАРОЛЬ]");
            return;
        }

        string password = words[1];

        if (string.Equals(password, _botConfiguration.Password) == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Неверный пароль.");
            return;
        }

        this.User.Role = BotConstants.UserRoles.Admin;
        this.User.AdditionalProperties.Set(BotConstants.AdminProperties.LastPasswordProperty, password);
        await this.BotDbContext.SaveChangesAsync();
        await BotClient.SendTextMessageAsync(Chat.ChatId, "Успешно.\n" +
                                                              "Вы получили роль администратора бота.\n" +
                                                              $"Список команд администратора {InfoAdminCommand.ADMININFO}");
    }
}