/// <summary>
/// Группа эндпоинтов для управления исключениями ботов.
/// Содержит все эндпоинты, связанные с обработкой и получением исключений.
/// </summary>

using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotException;

/// <summary>
/// Группа эндпоинтов для управления исключениями ботов.
/// Настраивает базовый путь "bot-exception" для всех эндпоинтов в группе.
/// </summary>
public sealed class ExceptionGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для управления исключениями
    /// </summary>
    public ExceptionGroup()
    {
        Configure("bot-exception", c =>
        {
        });
    }
}