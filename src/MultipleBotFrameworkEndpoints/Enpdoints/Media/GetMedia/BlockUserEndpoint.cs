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

sealed class GetMediaRequest 
{
    public long BotId { get; set; }
    public string FileId { get; set; }
}

sealed class GetMediaEndpoint : Endpoint<GetMediaRequest, List<BotUserEntity>>
{
    private BotDbContext _db;
    private IBotsManagerService _botsManagerService;

    public GetMediaEndpoint(BotDbContext db, IBotsManagerService botsManagerService)
    {
        _db = db;
        _botsManagerService = botsManagerService;
    }

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

    public override async Task HandleAsync(GetMediaRequest r, CancellationToken c)
    {
        BotEntity? bot = await _botsManagerService.GetBotById(r.BotId);

        if (bot is null) throw new Exception($"Не найден бот [{r.BotId}]");
        
        MyTelegramBotClient? botClient = await _botsManagerService.GetBotClientById(r.BotId);
        
        if(botClient is null) throw new Exception($"Не могу получить медиафайл. Бот не зарегистрирован [Id = {r.BotId}].");

        DownloadedTelegramFile telegramFile = await botClient.DownloadFileAsync(r.FileId);
        
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