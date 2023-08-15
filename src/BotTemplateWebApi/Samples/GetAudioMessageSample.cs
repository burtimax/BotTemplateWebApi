using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Controllers;
using BotFramework.Models;
using BotFramework.Other;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotTemplateWebApi.Samples;

[BotState("GetAudioMessageSample")]
public class GetAudioMessageSample : BaseBotState
{
    public GetAudioMessageSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Message.Type != MessageType.Voice)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Need voice");
            return;
        }

        // Можно сохранить файл локально на компьютер, а можно загрузить файл из серверов Telegram.
        
        // FilePath fp = new FilePath(Path.Combine(MediaPath, update.Message.Voice.FileUniqueId + ".burtimax"));
        // InputOnlineFile iof = await BotMediaHelper.GetFileByPath(fp); // Получаем файл из диска.

        var file = await BotMediaHelper.GetFileFromTelegramAsync(BotClient, update.Message.Voice.FileId); // Качаем файл из серверов Telegram.
        
        InputOnlineFile iof = new InputOnlineFile(file.fileData);
        
        await BotClient.SendVoiceAsync(
            chatId: Chat.ChatId,
            iof, "Hello");

        if (iof.Content != null) await iof.Content.DisposeAsync();

        await BotClient.SendTextMessageAsync(Chat.ChatId, "Вот держи свое голосовое обратно)");
    }
}