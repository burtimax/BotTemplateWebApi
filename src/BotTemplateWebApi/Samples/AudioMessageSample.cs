using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Models;
using BotFramework.Other;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotTemplateWebApi.Samples;

/// <summary>
/// Пример получения голосового сообщения и отправки его в ответ.
/// </summary>
/// <remarks>
/// Показываю, что можно получить файл 2 способами: из локального хранилища, из сервера Telegram.
/// </remarks>
[BotState("AudioMessageSample")]
public class AudioMessageSample : BaseBotState
{
    public AudioMessageSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message.Type != MessageType.Voice)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Жду голосовое от тебя.");
            return;
        }

        // Можно сохранить файл локально на компьютер, а можно загрузить файл из серверов Telegram.
        
        FilePath fp = new FilePath(Path.Combine(MediaDirectory, update.Message.Voice.FileUniqueId + ".burtimax"));
        Stream content = await BotMediaHelper.GetFileByPathAsync(fp);
        InputFileStream iofLocal = new InputFileStream(content); // Получаем файл из диска.

        var file = await BotMediaHelper.GetFileFromTelegramAsync(BotClient, update.Message.Voice.FileId); // Качаем файл из серверов Telegram.
        
        InputFile iofFromServer = new InputFileStream(file.fileData);
        
        await BotClient.SendVoiceAsync(
            chatId: Chat.ChatId,
            iofFromServer, 
            caption: "Hello");

        if (content != null) await content.DisposeAsync();

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Вот держи свое голосовое обратно)");
    }
}