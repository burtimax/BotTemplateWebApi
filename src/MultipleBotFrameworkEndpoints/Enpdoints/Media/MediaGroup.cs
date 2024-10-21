using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.Media;

public sealed class MediaGroup : SubGroup<BotApiGroup>
{
    public MediaGroup()
    {
        Configure("user", c =>
        {
            //c.Description(d => d.WithTags("user"));
        });
    }
}