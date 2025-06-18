/// <summary>
/// Корневая группа API для всех эндпоинтов ботов.
/// Содержит все подгруппы эндпоинтов, связанных с управлением ботами.
/// </summary>

using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints;

/// <summary>
/// Корневая группа API для всех эндпоинтов ботов.
/// Настраивает базовый путь "api" для всех эндпоинтов в системе.
/// </summary>
public class BotApiGroup : Group
{
    /// <summary>
    /// Инициализирует корневую группу API ботов
    /// </summary>
    public BotApiGroup()
    {
        Configure("api", d =>
        {
            
        });
    }
}