/// <summary>
/// Группа эндпоинтов для работы с медиа-файлами.
/// Содержит все эндпоинты, связанные с операциями над медиа-файлами.
/// </summary>

using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Media;

public sealed class MediaGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для работы с медиа-файлами.
    /// Настраивает базовый путь "user" для всех эндпоинтов в группе.
    /// </summary>
    public MediaGroup()
    {
        Configure("user", c =>
        {
            //c.Description(d => d.WithTags("user"));
        });
    }
}