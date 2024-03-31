using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Extensions;

public static class ITelegramBotClientExtensions
{
    public static async Task<int> SendSavedMessage(this ITelegramBotClient client, ChatId chatId,
        BotDbContext db, long savedMessageId)
    {
        List<BotSavedMessage> messages = await GetSavedMessages(db, savedMessageId) ??
                                         throw new Exception($"Not found in database saved_message");

        BotSavedMessage messageToSend = messages.First();
        var mesId = await client.CopyMessageAsync(chatId, messageToSend.TelegramChatId!,
            (int) messageToSend.TelegramMessageId!);

        return mesId.Id;
    }

    private static async Task<List<BotSavedMessage>?> GetSavedMessages(BotDbContext db, long savedMessageId)
    {
        BotSavedMessage? mes = await db.SavedMessages.FirstOrDefaultAsync(m => m.Id == savedMessageId);

        if (mes == null) return null;

        return await db.SavedMessages.Where(m => m.MediaGroupId == mes.MediaGroupId &&
                                           m.TelegramChatId == mes.TelegramChatId &&
                                           m.TelegramUserId == mes.TelegramUserId)
            .OrderBy(m => m.Id)
            .ToListAsync();
    }

    // private static Task<int> SendMediaGroup(ITelegramBotClient client, ChatId chatId, List<BotSavedMessage> messages)
    // {
    //     List<IAlbumInputMedia> mediaGroup = new List<IAlbumInputMedia>();
    //
    //     mediaGroup.Add(new InputMediaPhoto();
    //     
    //     client.SendMediaGroupAsync()
    // }
    //
    // private static GetInputFileFromMessage
}