using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFrameworkUpgrade.Extensions.ITelegramApiClient;

public static partial class ITelegramBotClientExtensions
{
    public static async Task<int> SendSavedMessageByCopy(this ITelegramBotClient client, long chatId, BotDbContext db, long savedMessageId)
    {
        List<BotSavedMessageEntity> messages = await GetSavedMessages(db, savedMessageId) ??
                                         throw new Exception($"Not found in database saved_message");
    
        BotSavedMessageEntity messageEntityToSend = messages.First();
        var mesId = await client.CopyMessageAsync(chatId, messageEntityToSend.TelegramChatId!.Value, (int) messageEntityToSend.TelegramMessageId!);
    
        return mesId.Id;
    }
    
    public static async Task SendSavedMessage(this ITelegramBotClient client, long chatId,
        BotDbContext db, long savedMessageId, ReplyMarkup replyMarkup = null)
    {
        List<BotSavedMessageEntity> messages = await GetSavedMessages(db, savedMessageId) ??
                                         throw new Exception($"Not found in database saved_message");
    
        if (messages.Count > 1)
        {
            await SendMediaGroup(client, chatId, messages);
        }
        else
        {
            await SendOneMessage(client, chatId, messages.First(), replyMarkup);
        }
    }

    private static async Task<List<BotSavedMessageEntity>?> GetSavedMessages(BotDbContext db, long savedMessageId)
    {
        BotSavedMessageEntity? mes = await db.SavedMessages.FirstOrDefaultAsync(m => m.Id == savedMessageId);

        if (mes == null) return null;

        return await db.SavedMessages.Where(m => m.MediaGroupId == mes.MediaGroupId &&
                                           m.TelegramChatId == mes.TelegramChatId &&
                                           m.TelegramUserId == mes.TelegramUserId)
            .OrderBy(m => m.Id)
            .ToListAsync();
    }
    
}