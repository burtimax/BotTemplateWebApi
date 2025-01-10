using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints.BotMethod;

public sealed class ProjectGroup : SubGroup<BotApiGroup>
{
    public ProjectGroup()
    {
        Configure("project", c =>
        {
            //c.Description(d => d.WithTags("bot"));
        });
    }
}