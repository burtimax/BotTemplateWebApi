/// <summary>
/// Эндпоинт для получения медиа-файлов из Telegram.
/// </summary>

using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.StaticFiles;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Extensions.ITelegramApiClient;
using MultipleBotFramework.Models;
using MultipleBotFramework.Services.Interfaces;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Media.GetMedia;

/// <summary>
/// Модель запроса для получения медиа-файла
/// </summary>
sealed class GetMediaRequest 
{
    /// <summary>
    /// Идентификатор бота
    /// </summary>
    public long BotId { get; set; }
    
    /// <summary>
    /// Идентификатор файла в Telegram
    /// </summary>
    public string FileId { get; set; }
}

/// <summary>
/// Эндпоинт для получения медиа-файлов из Telegram.
/// Скачивает файл из Telegram и отправляет его клиенту с правильными заголовками.
/// </summary>
sealed class GetMediaEndpoint : Endpoint<GetMediaRequest, List<BotUserEntity>>
{
    private BotDbContext _db;
    private IBotsManagerService _botsManagerService;

    /// <summary>
    /// Инициализирует эндпоинт получения медиа-файла
    /// </summary>
    /// <param name="db">Контекст базы данных</param>
    /// <param name="botsManagerService">Сервис управления ботами</param>
    public GetMediaEndpoint(BotDbContext db, IBotsManagerService botsManagerService)
    {
        _db = db;
        _botsManagerService = botsManagerService;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Get("/media/{BotId}/{FileId}");
        AllowAnonymous();
        Group<MediaGroup>();
        Summary(s =>
        {
            s.Summary = "Получить медиафайл";
            s.Description = $"Получить медиафайл";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение медиа-файла
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    /// <exception cref="Exception">Выбрасывается при отсутствии бота или невозможности получить клиент бота</exception>
    public override async Task HandleAsync(GetMediaRequest r, CancellationToken c)
    {
        BotEntity? bot = await _botsManagerService.GetBotById(r.BotId);

        if (bot is null) throw new Exception($"Не найден бот [{r.BotId}]");
        
        MyTelegramBotClient? botClient = await _botsManagerService.GetBotClientById(r.BotId);
        
        if(botClient is null) throw new Exception($"Не могу получить медиафайл. Бот не зарегистрирован [Id = {r.BotId}].");

        DownloadedTelegramFile telegramFile = await botClient.DownloadFileAsync(r.FileId);
        
        // Если у телеграм файла нет расширения, то расширение будет png по умолчанию.
        if (string.IsNullOrEmpty(Path.GetExtension(telegramFile.FileName)))
        {
            telegramFile.FileNameWithExtension = telegramFile.FileName + ".png";
            telegramFile.FileExtension = ".png";
        }
        
        var fileContentTypeProvider = new FileExtensionContentTypeProvider();
        string mimeType = fileContentTypeProvider.TryGetContentType(telegramFile.FileNameWithExtension, out string mime)
            ? mime
            : "application/octet-stream";
        
        HttpContext.Response.Headers.Append("Content-Disposition", "inline; filename=" + telegramFile.FileNameWithExtension);
        //Response.Headers.Append("Content-Disposition", "inline; filename=" + filename);
        using (MemoryStream ms = new(telegramFile.FileData))
        {
            await SendStreamAsync(
                stream: ms,
                fileName: null,
                fileLengthBytes: ms.Length,
                contentType: mimeType);

            return;
        }
    }
}