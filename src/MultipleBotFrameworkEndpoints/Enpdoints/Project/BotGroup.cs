/// <summary>
/// Группа эндпоинтов для управления проектами.
/// Содержит все эндпоинты, связанные с операциями над проектами.
/// </summary>

using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;

public sealed class ProjectGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для управления проектами.
    /// Настраивает базовый путь "project" для всех эндпоинтов в группе.
    /// </summary>
    public ProjectGroup()
    {
        Configure("project", c =>
        {
            //c.Description(d => d.WithTags("bot"));
        });
    }
}