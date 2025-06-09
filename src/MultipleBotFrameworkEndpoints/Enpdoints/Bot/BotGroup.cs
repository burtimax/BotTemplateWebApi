/// <summary>
/// Группа эндпоинтов для управления ботами.
/// Содержит все эндпоинты, связанные с операциями CRUD для ботов.
/// </summary>
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using MultipleBotFrameworkEndpoints.Enpdoints;

namespace MultipleTestBot.Endpoints.Bot;

public sealed class BotGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для управления ботами.
    /// Настраивает базовый путь "bot" для всех эндпоинтов в группе.
    /// </summary>
    public BotGroup()
    {
        Configure("bot", c =>
        {
            //c.Description(d => d.WithTags("bot"));
        });
    }
}