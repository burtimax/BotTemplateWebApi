/// <summary>
/// Эндпоинт для получения информации о проекте.
/// </summary>

using System.ComponentModel;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Extensions.ITelegramApiClient;
using MultipleBotFramework.Models;
using MultipleBotFramework.Options;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;
using MultipleTestBot.Endpoints.Bot;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;

/// <summary>
/// Модель запроса для получения информации о проекте
/// </summary>
public class GetProjectRequest
{
    /// <summary>
    /// Ключ проекта для идентификации
    /// </summary>
    public string Key { get; set; }
}

/// <summary>
/// Модель ответа с информацией о проекте
/// </summary>
public class GetProjectResponse
{
    /// <summary>
    /// Название проекта
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Токен доступа к проекту
    /// </summary>
    public string Token { get; set; }
}

/// <summary>
/// Эндпоинт для получения информации о проекте.
/// Возвращает название проекта и токен доступа.
/// </summary>
public class GetProjectEndpoint : Endpoint<GetProjectRequest, GetProjectResponse>
{
    private readonly BotConfiguration _config;

    /// <summary>
    /// Инициализирует эндпоинт получения информации о проекте
    /// </summary>
    /// <param name="config">Конфигурация бота</param>
    public GetProjectEndpoint(BotConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Настраивает параметры эндпоинта
    /// </summary>
    public override void Configure()
    {
        Post("/get");
        AllowAnonymous();
        Group<ProjectGroup>();
        Summary(s =>
        {
            s.Summary = "Получить информацию по проекту";
            s.Description = "Получить информацию по проекту";
        });
    }

    /// <summary>
    /// Обрабатывает запрос на получение информации о проекте
    /// </summary>
    /// <param name="r">Данные запроса</param>
    /// <param name="c">Токен отмены</param>
    public override async Task HandleAsync(GetProjectRequest r, CancellationToken c)
    {
        if (string.IsNullOrEmpty(r.Key))
        {
            await SendErrorsAsync();
            return;
        }
        
        await SendAsync(new GetProjectResponse()
        {
            Name = _config.Name ?? $"Проект {new Random().Next().ToString()}",
            Token = "BEARER TOKEN"
        });
    }
}

