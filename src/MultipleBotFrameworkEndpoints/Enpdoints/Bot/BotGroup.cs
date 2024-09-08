using FastEndpoints;
using Microsoft.AspNetCore.Http;
using MultipleBotFrameworkEndpoints.Enpdoints;

namespace MultipleTestBot.Endpoints.Bot;

public sealed class BotGroup : SubGroup<BotApiGroup>
{
    public BotGroup()
    {
        Configure("bot", c =>
        {
            //c.Description(d => d.WithTags("bot"));
        });
    }
}