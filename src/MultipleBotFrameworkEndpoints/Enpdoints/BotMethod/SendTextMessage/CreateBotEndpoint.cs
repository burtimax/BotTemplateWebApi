using System.ComponentModel;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Extensions.ITelegramApiClient;
using MultipleBotFramework.Models;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;
using MultipleTestBot.Endpoints.Bot;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

public class SendTextMessageRequest
{
    /// <summary>
    /// ИД сущности чата.
    /// </summary>
    public long ChatId { get; set; }
    
    /// <summary>
    /// HTML текст сообщения.
    /// </summary>
    public string Text { get; set; }
}

public class SendTextMessageEndpoint : Endpoint<SendTextMessageRequest, BotChatHistoryEntity>
{
    private IBotsManagerService _botsManagerService;
    private BotDbContext _db;

    public SendTextMessageEndpoint(IBotsManagerService botsManagerService, BotDbContext db)
    {
        _botsManagerService = botsManagerService;
        _db = db;
    }

    public override void Configure()
    {
        Post("/send-text-message");
        AllowAnonymous();
        Group<BotMethodGroup>();
    }

    public override async Task HandleAsync(SendTextMessageRequest r, CancellationToken c)
    {
        if (string.IsNullOrEmpty(r.Text.Trim(' '))) throw new Exception("Текст сообщения не должен быть пустой");
        
        BotChatEntity? chat = await _db.Chats.FirstOrDefaultAsync(c => c.Id == r.ChatId);

        if (chat is null) throw new Exception($"Не найден чат [Id = {r.ChatId}]");

        BotEntity? bot = await _botsManagerService.GetBotById(chat.BotId);

        if (bot is null) throw new Exception($"Не могу отправить сообщение. Бот не зарегистрирован [Id = {chat.BotId}].");

        MyTelegramBotClient? botClient = await _botsManagerService.GetBotClientById(chat.BotId);
        
        if(botClient is null) throw new Exception($"Не могу отправить сообщение. Бот не зарегистрирован [Id = {chat.BotId}].");
        
        var message = await botClient.SendMessageAsync(chatId: chat.ChatId, text: r.Text, parseMode: ParseMode.Html);

        var historyItem = await _db.ChatHistory.FirstOrDefaultAsync(h => h.BotId == chat.BotId && h.MessageId == message.MessageId);

        if (historyItem is not null)
        {
            await SendAsync(historyItem);
            return;
        }
        else
        {
            BotChatHistoryEntity item = new();
            message.TrySetContentToChatHistory(ref item);
            await SendAsync(item);
            return;
        }
    }
}

