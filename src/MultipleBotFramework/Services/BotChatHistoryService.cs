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

    public async Task<BotChatHistoryEntity?> SaveInChatHistoryIfNeed(long botId, long telegramChatId, bool isBot, object obj)
    {
        BotChatHistoryEntity item = new()
        {
            BotId = botId,
            TelegramChatId = telegramChatId,
            IsBot = isBot,
            JsonObject = obj?.ToJson(),
        };

        bool saveItem = false;

        if (obj is Update update)
        {
            if (update.TrySetContentToChatHistory(ref item))
            {
                saveItem = true;
            }
        }
        
        if (obj is Message mes)
        {
            item.Type = ChatHistoryType.Message;
            if (mes.TrySetContentToChatHistory(ref item))
            {
                saveItem = true;
            }
        }

        if (obj is MessageId mesId)
        {
            item.Type = ChatHistoryType.Message;
            item.Content = $"#[Бот прислал скопированное сообщение] {mesId.Id}";
            item.MessageId = mesId.Id;
            saveItem = true;
        }

        if (obj is IEnumerable<MessageId> messageIds)
        {
            item.Type = ChatHistoryType.Message;
            item.Content = $"#[Бот прислал копии сообщений] {string.Join(',', messageIds)}";
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
}