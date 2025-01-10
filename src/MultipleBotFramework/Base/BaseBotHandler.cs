using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.UpdatingMessages;

namespace MultipleBotFramework.Base;

/// <summary>
/// Базовый обработчик запроса бота.
/// </summary>
/// <typeparam name="TResuorces">Тип класса ресурсов. Строки, пути к файлам и т.д.</typeparam>
public class BaseBotHandler : ControllerBase, IBaseBotHandler
{
    /// <inheritdoc />
    public IServiceProvider ServiceProvider { get; set; }

    /// <inheritdoc />
    public long BotId { get; set; }

    /// <inheritdoc/>
    public BotUserEntity? User { get; set; }

    /// <inheritdoc/>
    public BotChatEntity? Chat { get; set; }
    
    /// <inheritdoc/>
    public Update Update { get; set; }

    /// <inheritdoc/>
    public BotDbContext BotDbContext { get; set; }

    /// <inheritdoc/>
    public string MediaDirectory { get; set; }

    /// <inheritdoc/>
    public ITelegramBotClient BotClient { get; set; }

    /// <inheritdoc/>
    public IReadOnlyList<ClaimValue>? UserClaims { get; set; }

    /// <inheritdoc />
    public bool IsOwner { get; set; }
    
    // Дополнительные свойства для удобства.
    protected string NotExpectedMessage { get; set; } 
    private readonly List<UpdateType> ExpectedUpdates = new ();
    private readonly List<MessageType> ExpectedMessageTypes = new ();

    public BaseBotHandler(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        BotDbContext = serviceProvider.GetRequiredService<BotDbContext>();
        var botConfig = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        MediaDirectory = botConfig.MediaDirectory;
    }

    /// <inheritdoc />
    public virtual Task SendIntroduction()
    {
        return Task.CompletedTask;
    }
    
    public virtual async Task HandleBotRequest(Update update)
    {
        if (IsExpectedUpdate(update) == false)
        {
            await UnexpectedUpdateHandler();
            return;
        }
        
        if (update.Type() == MultipleBotFramework.Enums.UpdateType.Message &&
            IsExpectedMessageType(update.Message) == false)
        {
            await UnexpectedUpdateHandler();
            return;
        }

        switch (update.Type())
        {
            case MultipleBotFramework.Enums.UpdateType.Message:
                await HandleMessage(update.Message!);
                break;
            case MultipleBotFramework.Enums.UpdateType.CallbackQuery:
                await HandleCallbackQuery(update.CallbackQuery!);
                break;
        }
    }

    public virtual async Task UnexpectedUpdateHandler()
    {
        try
        {
            await Answer(NotExpectedMessage);
        }
        catch (Exception e)
        {
        }
    }
    
    public virtual async Task HandleMessage(Message message)
    {
        throw new NotImplementedException();
    }
    
    public virtual async Task HandleCallbackQuery(CallbackQuery callbackQuery)
    {
        throw new NotImplementedException();
        return;
    }

    /// <summary>
    /// Заполняем ожидаемые типы запросов для состояния.
    /// </summary>
    /// <param name="types"></param>
    protected void Expected(params MultipleBotFramework.Enums.UpdateType[] types)
    {
        foreach (var type in types)
        {
            ExpectedUpdates.Add(type);
        }
    }

    /// <summary>
    /// Заполняем ожидаемые типы сообщений для состояния.
    /// </summary>
    /// <param name="types"></param>
    protected void ExpectedMessage(params MessageType[] types)
    {
        foreach (var type in types)
        {
            ExpectedMessageTypes.Add(type);
        }
    }
    
    private bool IsExpectedUpdate(Update update)
    {
        return ExpectedUpdates.Any() == false || ExpectedUpdates.Contains(update.Type());
    }
    
    private bool IsExpectedMessageType(Message message)
    {
        return ExpectedMessageTypes.Any() == false || ExpectedMessageTypes.Contains(message.Type());
    }

    
    protected virtual Task<Message> Answer(string text, string parseMode = ParseMode.Html, ReplyMarkup replyMarkup = default)
    {
        if (text.Length > BotConstants.Constraints.MaxMessageLength)
        {
            text = text.Substring(0, BotConstants.Constraints.MaxMessageLength - 1);
        }
        
        return BotClient.SendMessageAsync(Chat.ChatId, text:text, parseMode:parseMode, replyMarkup: replyMarkup);
    }
    
    protected virtual async Task AnswerCallback()
    {
        if (this.Update.Type() == MultipleBotFramework.Enums.UpdateType.CallbackQuery)
        {
            await BotClient.AnswerCallbackQueryAsync(this.Update.CallbackQuery.Id);
        }
    }
    
    protected virtual async Task DeleteMessage(int? messageId = null)
    {
        int? mesId = messageId == null ? Update?.Message?.MessageId : messageId;
        if(mesId is null) return;
        try
        {
            await BotClient.DeleteMessageAsync(Chat.ChatId, mesId.Value);
        }
        catch (Exception e) { }
    }
    
    protected virtual async Task ChangeState(string stateName, ChatStateSetterType setterType = ChatStateSetterType.ChangeCurrent)
    {
        Chat.States.Set(stateName, setterType);
        await BotDbContext.SaveChangesAsync();
    }
    
}