﻿using System.Collections;
using System.Collections.Generic;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework_Obsolete.Options;

public class BotConfiguration
{
    /// <summary>
    /// Наименование бота.
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Токен бота из Telegram.org.
    /// </summary>
    public string TelegramToken { get; set; }
        
    /// <summary>
    /// URL адрес, на который будут отправляться запросы к боту для обработки.
    /// </summary>
    public string Webhook { get; set; }

    /// <summary>
    /// Файл ресурсов бота
    /// </summary>
    public string? ResourcesFilePath { get; set; }
    
    /// <summary>
    /// Строка подключения к БД бота.
    /// </summary>
    public string DbConnection { get; set; }
    
    /// <summary>
    /// Пароль для команд бота.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Директория для хранения медиа файлов бота.
    /// </summary>
    public string MediaDirectory { get; set; }

    /// <summary>
    /// Директория для хранения файлов-отчетов ошибок бота.
    /// </summary>
    public string ExceptionDirectory { get; set; }

    public static IEnumerable<UpdateType> AllAllowedUpdates = new List<UpdateType>()
    {
        UpdateType.Message,
        UpdateType.CallbackQuery,
        UpdateType.ChannelPost,
        UpdateType.ChatMember,
        UpdateType.EditedMessage,
        UpdateType.ChatJoinRequest,
        UpdateType.MyChatMember,
        UpdateType.InlineQuery,
        UpdateType.ShippingQuery,
        UpdateType.PollAnswer,
        UpdateType.ChosenInlineResult,
        UpdateType.EditedChannelPost,
        UpdateType.PreCheckoutQuery,
        UpdateType.Poll
    };
}