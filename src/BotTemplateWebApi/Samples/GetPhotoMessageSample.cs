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

[BotState("GetPhotoMessageSample")]
public class GetPhotoMessageSample : BaseBotState
{
    public GetPhotoMessageSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message.Type != MessageType.Photo)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Жду фото от тебя.");
            return;
        }

        // Можно сохранить файл локально на компьютер, а можно загрузить файл из серверов Telegram.
        
        FilePath fp = new FilePath(Path.Combine(MediaPath, update.Message.Photo.GetFileByQuality(PhotoQuality.Low).FileUniqueId + ".jpeg"));
        await BotMediaHelper.DownloadAndSaveTelegramFileAsync(BotClient,
            update.Message.Photo.GetFileByQuality(PhotoQuality.Low).FileId, fp);
        InputOnlineFile iof = new InputOnlineFile(await BotMediaHelper.GetFileByPathAsync(fp)); // Получаем файл из диска.

        var file = await BotMediaHelper.GetPhotoFromTelegramAsync(BotClient, PhotoQuality.Low, update.Message.Photo!); // Качаем файл из серверов Telegram.
        
        InputOnlineFile iofServer = new InputOnlineFile(file.fileData);
        
        await BotClient.SendPhotoAsync(
            chatId: Chat.ChatId,
            iof, "Hello");

        if (iof.Content != null) await iof.Content.DisposeAsync();

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Вот держи свое фото обратно в шакальном качестве)");
    }
}