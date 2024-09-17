using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotException;

public sealed class ExceptionGroup : SubGroup<BotApiGroup>
{
    public ExceptionGroup()
    {
        Configure("bot-exception", c =>
        {
        });
    }
}