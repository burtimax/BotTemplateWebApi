/// <summary>
/// Группа эндпоинтов для выполнения методов Telegram Bot API.
/// Содержит все эндпоинты, связанные с отправкой сообщений и другими методами API.
/// </summary>

using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;

/// <summary>
/// Группа эндпоинтов для выполнения методов Telegram Bot API.
/// Настраивает базовый путь "bot-method" для всех эндпоинтов в группе.
/// </summary>
public sealed class BotMethodGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для методов бота
    /// </summary>
    public BotMethodGroup()
    {
        Configure("bot-method", c =>
        {
            //c.Description(d => d.WithTags("bot"));
        });
    }
}