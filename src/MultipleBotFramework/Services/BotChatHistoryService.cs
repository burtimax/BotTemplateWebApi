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

public class BotChatHistoryService
{
    protected readonly BotDbContext _db;
    
    public BotChatHistoryService(BotDbContext db)
    {
        _db = db;
    }

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