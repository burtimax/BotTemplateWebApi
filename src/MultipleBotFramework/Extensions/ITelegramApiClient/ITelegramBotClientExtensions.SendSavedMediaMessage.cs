using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Extensions.ITelegramApiClient;

public static partial class ITelegramBotClientExtensions
{
    
    private static async Task<IEnumerable<Message>> SendMediaGroup(ITelegramBotClient client, long chatId, List<BotSavedMessageEntity> messages)
    {
        List<InputMedia> mediaGroup = new List<InputMedia>();
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
    private static InputMedia GetMediaFromMessage(BotSavedMessageEntity messageEntity)
    {
        Message mes = messageEntity.TelegramMessage!;

        dynamic? media = messageEntity.TelegramMessage.Type() switch
        {
            MessageType.Photo => new InputMediaPhoto(messageEntity.TelegramMessage.Photo.Last().FileId),
            MessageType.Video => new InputMediaVideo(messageEntity.TelegramMessage.Video!.FileId),
            MessageType.Audio => new InputMediaAudio(messageEntity.TelegramMessage.Audio!.FileId),
            MessageType.Document => new InputMediaDocument(messageEntity.TelegramMessage.Document!.FileId),
            _ => null,
        };

        if (media == null) throw new NotImplementedException();

        media.Caption = mes.Caption;
        media.CaptionEntities = mes.CaptionEntities;

        return media;
    }
}