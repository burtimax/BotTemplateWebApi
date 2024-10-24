using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;

public sealed class BotMethodGroup : SubGroup<BotApiGroup>
{
    public BotMethodGroup()
    {
        Configure("bot-method", c =>
        {
            //c.Description(d => d.WithTags("bot"));
        });
    }
}