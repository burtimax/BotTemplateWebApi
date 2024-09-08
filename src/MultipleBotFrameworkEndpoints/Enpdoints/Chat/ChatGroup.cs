using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Chat;

public sealed class ChatGroup : SubGroup<BotApiGroup>
{
    public ChatGroup()
    {
        Configure("chat", c =>
        {
        });
    }
}