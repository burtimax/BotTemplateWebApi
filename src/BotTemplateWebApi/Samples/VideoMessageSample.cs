using BotFramework.Attributes;
using BotFramework.Base;
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
[BotState("VideoMessageSample")]
public class VideoMessageSample : BaseBotState
{
    public VideoMessageSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message.Type != MessageType.Video)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Жду видео от тебя.");
            return;
        }

        // Можно сохранить файл локально на компьютер, а можно загрузить файл из серверов Telegram.
        
        FilePath fp = new FilePath(Path.Combine(MediaPath, update.Message.Video.FileUniqueId + ".mp4"));
        await BotMediaHelper.DownloadAndSaveTelegramFileAsync(BotClient, update.Message.Video.FileId, fp);
        InputOnlineFile iofLocal = new InputOnlineFile(await BotMediaHelper.GetFileByPathAsync(fp)); // Получаем файл из диска.

        var file = await BotMediaHelper.GetFileFromTelegramAsync(BotClient, update.Message.Video.FileId); // Качаем файл из серверов Telegram.
        
        InputOnlineFile iofFromServer = new InputOnlineFile(file.fileData);
        
        await BotClient.SendVideoAsync(
            chatId: Chat.ChatId,
            iofLocal);

        if (iofFromServer.Content != null) await iofFromServer.Content.DisposeAsync();
        if (iofLocal.Content != null) await iofLocal.Content.DisposeAsync();

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Вот держи свое видео обратно)");
    }
}