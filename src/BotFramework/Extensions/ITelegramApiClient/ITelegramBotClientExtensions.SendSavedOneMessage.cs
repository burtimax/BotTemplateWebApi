using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Enums;
using BotFramework.Other;
using BotFramework.Services;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Extensions;

public static partial class ITelegramBotClientExtensions
{
    private static async Task SendOneMessage(ITelegramBotClient client, ChatId chatId, BotSavedMessage message)
    {
        Message mes = message.TelegramMessage!;
        
        switch (mes.Type)
        {
            case MessageType.Animation:
                await SendAnimation(client, chatId, mes);
                return;
            case MessageType.Audio:
                await SendAudio(client, chatId, mes);
                return;
            case MessageType.Text:
                await SendText(client, chatId, mes);
                return;
            case MessageType.Photo:
                await SendPhoto(client, chatId, mes);
                return;
            case MessageType.Voice:
                await SendVoice(client, chatId, mes);
                return;
            case MessageType.Sticker:
                await SendSticker(client, chatId, mes);
                return;
            case MessageType.Video:
                await SendVideo(client, chatId, mes);
                return;
            case MessageType.VideoNote:
                await SendVideoNote(client, chatId, mes);
                return;
            case MessageType.Document:
                await SendDocument(client, chatId, mes);
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
    private static async Task SendDocument(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Document!.FileId);
        await client.SendDocumentAsync(
            chatId:chatId,
            document: input,
            caption:mes.Caption,
            captionEntities: mes.CaptionEntities
        );
    }
    
    /// <summary>
    /// Отправить кружочек.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendVideoNote(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.VideoNote!.FileId);
        await client.SendVideoNoteAsync(
            chatId:chatId,
            videoNote:input
        );
    }
    
    /// <summary>
    /// Отправить видео.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendVideo(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Video!.FileId);
        await client.SendVideoAsync(
            chatId:chatId,
            video: input,
            caption: mes.Caption,
            captionEntities:mes.CaptionEntities
            );
    }
    
    /// <summary>
    /// Отправить стикер.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendSticker(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Sticker!.FileId);
        await client.SendStickerAsync(
            chatId:chatId,
            sticker:input,
            emoji:mes.Sticker.Emoji);
    }
    
    /// <summary>
    /// Отправить голосовое сообщение.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendVoice(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Voice!.FileId);
        await client.SendVoiceAsync(
            chatId:chatId,
            voice: input,
            duration: mes.Voice.Duration,
            caption: mes.Caption,
            captionEntities: mes.CaptionEntities);
    }
    
    /// <summary>
    /// Отправить фото.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendPhoto(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Photo![^1].FileId);
        var message = await client.SendPhotoAsync(
            chatId:chatId,
            photo:input,
            caption: mes.Caption,
            captionEntities: mes.CaptionEntities,
            hasSpoiler: mes.HasMediaSpoiler);
        int c = 1;
    }
    
    /// <summary>
    /// Отправить текст.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendText(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        await client.SendTextMessageAsync(
            chatId:chatId, 
            text: mes.Text!,
            entities: mes.Entities,
            disableWebPagePreview: true
            );
    }

    /// <summary>
    /// Отправить аудио.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendAudio(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Audio!.FileId);
        await client.SendAudioAsync(
            chatId:chatId, 
            audio: input, 
            caption:mes.Caption,
            captionEntities:mes.CaptionEntities,
            duration: mes.Audio.Duration);
    }
    
    /// <summary>
    /// Отправить анимацию.
    /// </summary>
    /// <param name="client">Telegram Bot API клиент.</param>
    /// <param name="chatId">ИД чата.</param>
    /// <param name="mes">Сообщение</param>
    private static async Task SendAnimation(ITelegramBotClient client, ChatId chatId, Message mes)
    {
        InputFile input = InputFile.FromString(mes.Animation!.FileId);
        await client.SendAnimationAsync(
            chatId:chatId, 
            animation:input, 
            caption:mes.Caption,
            captionEntities:mes.CaptionEntities);
    }
}