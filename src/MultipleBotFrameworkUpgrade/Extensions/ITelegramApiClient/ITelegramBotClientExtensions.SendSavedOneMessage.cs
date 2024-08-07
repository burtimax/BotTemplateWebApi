using System;
using System.Linq;
using System.Threading.Tasks;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Enums;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Stickers;

namespace MultipleBotFrameworkUpgrade.Extensions.ITelegramApiClient;

public static partial class ITelegramBotClientExtensions
{
    private static async Task SendOneMessage(ITelegramBotClient client, long chatId, BotSavedMessageEntity messageEntity, ReplyMarkup replyMarkup = default)
    {
        Message mes = messageEntity.TelegramMessage!;
        
        switch (mes.Type())
        {
            case MessageType.Animation:
                await SendAnimation(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Audio:
                await SendAudio(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Text:
                await SendText(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Photo:
                await SendPhoto(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Voice:
                await SendVoice(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Sticker:
                await SendSticker(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Video:
                await SendVideo(client, chatId, mes, replyMarkup);
                return;
            case MessageType.VideoNote:
                await SendVideoNote(client, chatId, mes, replyMarkup);
                return;
            case MessageType.Document:
                await SendDocument(client, chatId, mes, replyMarkup);
                return;
            default: throw new NotImplementedException();
        }
    }
    
    /// <summary>
    /// Отправить документ.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendDocument(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendDocumentAsync(
            chatId:chatId,
            document: mes.Document!.FileId,
            caption:mes.Caption,
            captionEntities: mes.CaptionEntities,
            replyMarkup: replyMarkup
        );
    }
    
    /// <summary>
    /// Отправить кружочек.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendVideoNote(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        
        await client.SendVideoNoteAsync(
            chatId:chatId,
            videoNote:mes.VideoNote!.FileId,
            replyMarkup: replyMarkup
        );
    }
    
    /// <summary>
    /// Отправить видео.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendVideo(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendVideoAsync(
            chatId:chatId,
            video: mes.Video!.FileId,
            caption: mes.Caption,
            captionEntities:mes.CaptionEntities,
            replyMarkup: replyMarkup
            );
    }
    
    /// <summary>
    /// Отправить стикер.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendSticker(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendStickerAsync(
            chatId:chatId,
            sticker:mes.Sticker!.FileId,
            emoji:mes.Sticker.Emoji,
            replyMarkup: replyMarkup
            );
    }
    
    /// <summary>
    /// Отправить голосовое сообщение.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendVoice(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendVoiceAsync(
            chatId:chatId,
            voice: mes.Voice!.FileId,
            duration: mes.Voice.Duration,
            caption: mes.Caption,
            captionEntities: mes.CaptionEntities,
            replyMarkup: replyMarkup
            );
    }
    
    /// <summary>
    /// Отправить фото.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendPhoto(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        var message = await client.SendPhotoAsync(
            chatId:chatId,
            photo:mes.Photo.Last().FileId,
            caption: mes.Caption,
            captionEntities: mes.CaptionEntities,
            hasSpoiler: mes.HasMediaSpoiler,
            replyMarkup: replyMarkup
            );
    }
    
    /// <summary>
    /// Отправить текст.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendText(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendMessageAsync(
            chatId:chatId, 
            text: mes.Text!,
            entities: mes.Entities,
            linkPreviewOptions: new LinkPreviewOptions(){IsDisabled = true},
            replyMarkup: replyMarkup
            );
    }

    /// <summary>
    /// Отправить аудио.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendAudio(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendAudioAsync(
            chatId:chatId, 
            audio: mes.Audio!.FileId, 
            caption:mes.Caption,
            captionEntities:mes.CaptionEntities,
            duration: mes.Audio.Duration,
            replyMarkup: replyMarkup
            );
    }
    
    /// <summary>
    /// Отправить анимацию.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendAnimation(ITelegramBotClient client, long chatId, Message mes, ReplyMarkup replyMarkup = default)
    {
        await client.SendAnimationAsync(
            chatId:chatId, 
            animation:mes.Animation!.FileId, 
            caption:mes.Caption,
            captionEntities:mes.CaptionEntities,
            replyMarkup: replyMarkup
            );
    }
}