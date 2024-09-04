using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;
using MultipleBotFrameworkUpgrade.Enums;
using MultipleBotFrameworkUpgrade.Extensions;
using MultipleBotFrameworkUpgrade.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.States.SaveMessage;

[BotState(Name)]
[BotHandler(stateName:Name)]
internal class SaveMessageHandler : BaseBotHandler
{
    public const string Name = "__saveMessageBaseState__";
    
    private readonly BotDbContext _db;
    private readonly ISavedMessageService _savedMessageService;
    
    public SaveMessageHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _db = serviceProvider.GetRequiredService<BotDbContext>();
        _savedMessageService = serviceProvider.GetRequiredService<ISavedMessageService>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (update.Type() != UpdateType.Message)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Ожидается тип [cообщение]!\n" +
                                                              "Сохранение отменено.");
            Chat.States.GoBack(ChatStateGoBackType.GoToPrevious);
            await _db.SaveChangesAsync();
        }

        if (GetSupportedMessageTypes()
                .Contains(update.Message.Type()) == false)
        {
            await BotClient.SendMessageAsync(Chat.ChatId, "Не поддерживается такой тип сообщения для сохранения!\n" +
                                                              "Сохранение отменено.");
            Chat.States.GoBack(ChatStateGoBackType.GoToPrevious);
            await _db.SaveChangesAsync();
        }
        
        BotSavedMessageEntity savedMessageEntity = await _savedMessageService.SaveMessageFromUpdate(BotId, Chat, User, update.Message!);
        
        await BotClient.SendMessageAsync(Chat.ChatId, $"Сообщение сохранено.\nИД = [{savedMessageEntity.Id}]");
        
        Chat.States.GoBack(ChatStateGoBackType.GoToPrevious);
        await _db.SaveChangesAsync();
        return;
    }

    private List<MessageType> GetSupportedMessageTypes() => new List<MessageType>()
    {
        MessageType.Animation,
        MessageType.Audio,
        MessageType.Text,
        MessageType.Photo,
        MessageType.Voice,
        MessageType.Sticker,
        MessageType.Video,
        MessageType.VideoNote,
        MessageType.Document,
    };
}