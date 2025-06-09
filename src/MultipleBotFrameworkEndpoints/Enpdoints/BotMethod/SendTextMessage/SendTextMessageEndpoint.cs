/// <summary>
/// Эндпоинт для отправки текстового сообщения через бота.
/// </summary>

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

/// <summary>
/// Модель запроса для отправки текстового сообщения
/// </summary>
public class SendTextMessageRequest
{
    /// <summary>
    /// ID чата для отправки сообщения
    /// </summary>
    public long ChatId { get; set; }
    
    /// <summary>
    /// Текст сообщения в формате HTML
    /// </summary>
    public string Text { get; set; }
}

/// <summary>
/// Эндпоинт для отправки текстового сообщения через бота.
/// Позволяет отправить сообщение от имени бота в указанный чат.
/// Сообщение сохраняется в истории чата.
/// </summary>
public class SendTextMessageEndpoint : Endpoint<SendTextMessageRequest, BotChatHistoryEntity>
{
    private IBotsManagerService _botsManagerService;
    private BotDbContext _db;

    /// <summary>
    /// Инициализирует эндпоинт отправки текстового сообщения
    /// </summary>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    /// <param name="db">Контекст базы данных</param>
    public SendTextMessageEndpoint(IBotsManagerService botsManagerService, BotDbContext db)
    {
        _botsManagerService = botsManagerService;
        _db = db;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Post("/send-text-message");
        AllowAnonymous();
        Group<BotMethodGroup>();
        Summary(s =>
        {
            s.Summary = "Отправить сообщение через бота чату (пользователю).";
            s.Description = "Отправить сообщение от лица бота в чат. Общаемся через бота.";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на отправку текстового сообщения
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    /// <exception cref="Exception">Выбрасывается при пустом тексте сообщения, если чат не найден или бот не зарегистрирован</exception>
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

