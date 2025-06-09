/// <summary>
/// Группа эндпоинтов для управления чатами.
/// Содержит все эндпоинты, связанные с операциями над чатами.
/// </summary>

using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat;

/// <summary>
/// Группа эндпоинтов для управления чатами.
/// Настраивает базовый путь "chat" для всех эндпоинтов в группе.
/// </summary>
public sealed class ChatGroup : SubGroup<BotApiGroup>
{
    /// <summary>
    /// Инициализирует группу эндпоинтов для управления чатами
    /// </summary>
    public ChatGroup()
    {
        Configure("chat", c =>
        {
        });
    }
}