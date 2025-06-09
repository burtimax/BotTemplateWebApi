using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils.Keyboard;
using Newtonsoft.Json;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Services;

/// <summary>
/// Сервис для сохранения истории чатов и сообщений бота.
/// </summary>
public class BotChatHistoryService
{
    protected readonly BotDbContext _db;
    
    /// <summary>
    /// Конструктор сервиса истории чатов.
    /// </summary>
    /// <param name="db">Контекст базы данных бота</param>
    public BotChatHistoryService(BotDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Сохраняет событие или сообщение в истории чата, если это необходимо.
    /// </summary>
    /// <param name="botId">ID бота</param>
    /// <param name="telegramChatId">ID чата</param>
    /// <param name="isBot">Признак, что сообщение от бота</param>
    /// <param name="data">Данные для сохранения</param>
    /// <param name="args">Дополнительные аргументы</param>
    /// <returns>Сущность истории чата или null</returns>
    public async Task<BotChatHistoryEntity?> SaveInChatHistoryIfNeed(long botId, long telegramChatId, bool isBot, object data, object? args = null)
    {
        var inline = GetInlineKeyboardFromResult(data);
        var inlineData = inline != null ? new{ReplyMarkup = inline}.ToJson() : null;
        var reply = GetReplyKeyboardFromArgs(args);
        var replyData = reply != null ? new{ReplyMarkup = reply}.ToJson() : null;
        
        BotChatHistoryEntity item = new()
        {
            BotId = botId,
            TelegramChatId = telegramChatId,
            IsBot = isBot,
            JsonData = data?.ToJson(),
            ReplyKeyboard = replyData,
            InlineKeyboard = inlineData,
            ReplyToMessageId = GetReplyToMessageIdFromData(data),
        };

        bool saveItem = false;

        if (data is Update update)
        {
            if (update.TrySetContentToChatHistory(ref item))
            {
                saveItem = true;
            }
        }
        
        if (data is Message mes)
        {
            item.Type = ChatHistoryType.Message;
            if (mes.TrySetContentToChatHistory(ref item))
            {
                saveItem = true;
            }
        }

        if (data is MessageId mesId)
        {
            item.Type = ChatHistoryType.Message;
            item.Text = $"#[Бот прислал скопированное сообщение] {mesId.Id}";
            item.MessageId = mesId.Id;
            saveItem = true;
        }

        if (data is IEnumerable<MessageId> messageIds)
        {
            item.Type = ChatHistoryType.Message;
            item.Text = $"#[Бот прислал копии сообщений] {string.Join(',', messageIds)}";
            item.MessageId = messageIds.Last().Id;
            saveItem = true;
        }

        if (saveItem)
        {
            _db.ChatHistory.Add(item);
            await _db.SaveChangesAsync();
            return item;
        }

        return null;
    }

    /// <summary>
    /// Получить reply-клавиатуру из аргументов.
    /// </summary>
    private IEnumerable<IEnumerable<KeyboardButton>>? GetReplyKeyboardFromArgs(object? args)
    {
        if (args is not null 
            && args is IDictionary<string, object> dict)
        {
            if (dict.ContainsKey(PropertyNames.ReplyMarkup) 
                && dict[PropertyNames.ReplyMarkup] is ReplyKeyboardMarkup replyMarkup)
            {
                return replyMarkup.Keyboard;
            }
        }

        return null;
    }

    /// <summary>
    /// Получить ID сообщения, на которое был ответ.
    /// </summary>
    private int? GetReplyToMessageIdFromData(object data)
    {
        if (data is not null && data is Update update
                             && update.Message is not null
                             && update.Message.ReplyToMessage is not null)
        {
            return update.Message.ReplyToMessage.MessageId;
        }

        return null;
    }
    
    /// <summary>
    /// Получить inline-клавиатуру из результата.
    /// </summary>
    private IEnumerable<IEnumerable<InlineKeyboardButton>>? GetInlineKeyboardFromResult(object? result)
    {
        if (result is not null 
            && result is Message mes)
        {
            return mes.ReplyMarkup?.InlineKeyboard;
        }

        return null;
    }
}