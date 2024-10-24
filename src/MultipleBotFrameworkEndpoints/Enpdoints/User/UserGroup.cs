using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace MultipleBotFrameworkEndpoints.Enpdoints.User;

public sealed class UserGroup : SubGroup<BotApiGroup>
{
    public UserGroup()
    {
        Configure("user", c =>
        {
            //c.Description(d => d.WithTags("user"));
        });
    }
}