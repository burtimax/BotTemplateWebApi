using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MultipleBotFramework.Db.Entity;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework.Extensions.ITelegramApiClient;

public static partial class ITelegramBotClientExtensions
{
    
    private static async Task<Message[]> SendMediaGroup(ITelegramBotClient client, ChatId chatId, List<BotSavedMessageEntity> messages)
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
    /// <param name="messageEntity"></param>
    /// <returns></returns>
    private static IAlbumInputMedia GetMediaFromMessage(BotSavedMessageEntity messageEntity)
    {
        Message mes = messageEntity.TelegramMessage!;

        dynamic? media = messageEntity.TelegramMessage.Type switch
        {
            MessageType.Photo => new InputMediaPhoto(InputFile.FromString(messageEntity.TelegramMessage.Photo![^1].FileId)),
            MessageType.Video => new InputMediaVideo(InputFile.FromString(messageEntity.TelegramMessage.Video!.FileId)),
            MessageType.Audio => new InputMediaAudio(InputFile.FromString(messageEntity.TelegramMessage.Audio!.FileId)),
            MessageType.Document => new InputMediaDocument(InputFile.FromString(messageEntity.TelegramMessage.Document!.FileId)),
            _ => null,
        };

        if (media == null) throw new NotImplementedException();

        media.Caption = mes.Caption;
        media.CaptionEntities = mes.CaptionEntities;

        return media;
    }
}