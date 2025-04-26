using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Db;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Options;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;
using MultipleBotFramework.Utils.Keyboard;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда уведомления (тест для проверки).
/// </summary>
[BotCommand(Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
[BotHandler(command: Name, requiredUserClaims: new []{ BotConstants.BaseBotClaims.BotUserNotificationSend })]
public class NotifyTestCommand : BaseBotHandler
{
    public const string Name = "/testnotify";
    
    public NotifyTestCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message?.ReplyToMessage == null)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Ответь на какое-нибудь сообщение");
            return;
        }
        
        string text = update.Message.Text!.Replace(Name, "").Trim(' ');

        bool parsed = TryParseInlineKeyboard(text, out string? error, out var kb);
        
        if (error != null)
        {
            await Answer(error);
            return;
        }
        
        if (parsed)
        {
            // Не будем ждать окончания работы.
            await BotClient.CopyMessageAsync(Chat.ChatId, Chat.ChatId,
                update.Message.ReplyToMessage.MessageId, replyMarkup: kb?.Build());
        }
        else
        {
            await Answer("В норм формате пришли мне, а?");
        }
    }

    /// <summary>
    /// [url::Название кнопки::https://www.youtube.com/][act::Название кнопки::callback_data]
    /// </summary>
    /// <param name="input"></param>
    /// <param name="errorStr"></param>
    /// <param name="kb"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    protected bool TryParseInlineKeyboard(string input, out string? errorStr, out InlineKeyboardBuilder? kb)
    {
        kb = new();
        errorStr = null;
        
        if (string.IsNullOrEmpty(input))
        {
            kb = null;
            return true;
        }
        
        try
        {
            // Разбиваем строку на отдельные команды
            var commands = input.Split(new[] { "][" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var command in commands)
            {
                // Убираем квадратные скобки
                var cleanCommand = command.Trim('[', ']');
                var parts = cleanCommand.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 3)
                {
                    string type = parts[0];
                    string text = parts[1];
                    string action = parts[2];

                    InlineKeyboardButton button;

                    if (type == "url")
                    {
                        button = new InlineKeyboardButton(text)
                        {
                            Url = action
                        };
                    }
                    else if (type == "act")
                    {
                        button = new InlineKeyboardButton(text)
                        {
                            CallbackData = action
                        };
                    }
                    else
                    {
                        throw new ArgumentException($"Неизвестный тип кнопки: {type}");
                    }

                    kb.NewRow().Add(button);
                }
                else
                {
                    errorStr = $"Неизвестный тип кнопки: [url] или [act]\n" +
                               $"Должно быть:\n" +
                               $"<code>[url::Название кнопки::https://www.youtube.com/]</code> или <code>[act::Название кнопки::callback_data]</code>";
                    return false;
                }
            }
        }
        catch (Exception e)
        {
            errorStr = "Неправильный формат кнопок.\n" +
                       "Должно быть:\n" +
                       "<code>[url::Название кнопки::https://www.youtube.com/]</code> или <code>[act::Название кнопки::callback_data]</code>";
            return false;
        }

        return true;
    }
}