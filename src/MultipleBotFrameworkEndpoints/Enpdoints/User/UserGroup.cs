/// <summary>
/// Группа эндпоинтов для управления пользователями.
/// Содержит все эндпоинты, связанные с операциями над пользователями.
/// </summary>

using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace MultipleBotFrameworkEndpoints.Enpdoints.User;

public sealed class UserGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для управления пользователями.
    /// Настраивает базовый путь "user" для всех эндпоинтов в группе.
    /// </summary>
    public UserGroup()
    {
        Configure("user", c =>
        {
            //c.Description(d => d.WithTags("user"));
        });
    }
}