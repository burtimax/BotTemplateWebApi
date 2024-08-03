﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда для регистрации админа в системе.
/// Админов может быть несколько.
/// Пример команды: [/auth {ПАРОЛЬ БОТА}]
/// </summary>
[BotCommand(Name)]
public class AuthCommand: BaseBotCommand
{
    internal const string Name = "/auth";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _botRepository;
    
    public AuthCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _botRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string[] words = update.Message.Text.Split(' ');
        if (words.Length < 2)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId,
                $"Команда не правильна, введите пароль админа.\n" +
                    $"Например: [{Name} ПАРОЛЬ]");
            return;
        }

        string password = words[1];

        if (string.Equals(password, _botConfiguration.Password) == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Неверный пароль.");
            return;
        }

        await _botRepository.AddClaimToUser(BotId, User.Id, BotConstants.BaseBotClaims.IAmBruceAlmighty);
        this.User.AdditionalProperties.Set(BotConstants.AdminProperties.LastPasswordProperty, password);
        await this.BotDbContext.SaveChangesAsync();
        await BotClient.SendTextMessageAsync(Chat.ChatId, "Успешно.\n" +
                                                              "Вы получили роль администратора бота.\n" +
                                                              $"Список команд администратора {CommandsCommand.Name}");
    }
}