using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Controllers;
using BotFramework.Enums;
using BotFramework.Extensions;
using BotFramework.Models;
using BotFramework.Other;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotTemplateWebApi.Samples;

/// <summary>
/// Пример получения видеофайла и отправки его в ответ.
/// </summary>
/// <remarks>
/// Показываю, что можно получить файл 2 способами: из локального хранилища, из сервера Telegram.
/// </remarks>
[BotState("VideoNoteMessageSample")]
public class VideoNoteMessageSample : BaseBotState
{
    public VideoNoteMessageSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message.Type != MessageType.VideoNote)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Жду видео от видео кружочек.");
            return;
        }

        // Можно сохранить файл локально на компьютер, а можно загрузить файл из серверов Telegram.
        
        FilePath fp = new FilePath(Path.Combine(MediaPath, update.Message.VideoNote.FileUniqueId + ".mp4"));
        await BotMediaHelper.DownloadAndSaveTelegramFileAsync(BotClient, update.Message.VideoNote.FileId, fp);
        InputOnlineFile iofLocal = new InputOnlineFile(await BotMediaHelper.GetFileByPathAsync(fp)); // Получаем файл из диска.

        var file = await BotMediaHelper.GetFileFromTelegramAsync(BotClient, update.Message.VideoNote.FileId); // Качаем файл из серверов Telegram.
        
        InputOnlineFile iofFromServer = new InputOnlineFile(file.fileData);
        
        await BotClient.SendVideoNoteAsync(
            chatId: Chat.ChatId,
            iofLocal);

        if (iofFromServer.Content != null) await iofFromServer.Content.DisposeAsync();
        if (iofLocal.Content != null) await iofLocal.Content.DisposeAsync();

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Вот держи свой кружочек обратно)");
    }
}