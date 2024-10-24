using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Extensions;

public static partial class ITelegramBotClientExtensions
{
    
    private static async Task<Message[]> SendMediaGroup(ITelegramBotClient client, ChatId chatId, List<BotSavedMessage> messages)
    {
        List<IAlbumInputMedia> mediaGroup = new List<IAlbumInputMedia>();
        string? caption = null;
        List<MessageEntity>? entities = null;
        
        foreach (var savedMessage in messages)
        {
            mediaGroup.Add(GetMediaFromMessage(savedMessage));
        }

        var result = await client.SendMediaGroupAsync(chatId, mediaGroup);
        return result;
    }

    /// <summary>
    /// Получить по сообщению необходимый медиафайл.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private static IAlbumInputMedia GetMediaFromMessage(BotSavedMessage message)
    {
        Message mes = message.TelegramMessage!;

        dynamic? media = message.TelegramMessage.Type switch
        {
            MessageType.Photo => new InputMediaPhoto(InputFile.FromString(message.TelegramMessage.Photo![^1].FileId)),
            MessageType.Video => new InputMediaVideo(InputFile.FromString(message.TelegramMessage.Video!.FileId)),
            MessageType.Audio => new InputMediaAudio(InputFile.FromString(message.TelegramMessage.Audio!.FileId)),
            MessageType.Document => new InputMediaDocument(InputFile.FromString(message.TelegramMessage.Document!.FileId)),
            _ => null,
        };

        if (media == null) throw new NotImplementedException();

        media.Caption = mes.Caption;
        media.CaptionEntities = mes.CaptionEntities;

        return media;
    }
}