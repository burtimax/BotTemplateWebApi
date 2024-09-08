using FastEndpoints;

namespace MultipleBotFrameworkEndpoints.Enpdoints;

public class BotApiGroup : Group
{
    public BotApiGroup()
    {
        Configure("api", d =>
        {
            
        });
    }
}