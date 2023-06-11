using BotTemplateWebApi.States;
using BotTemplateWebApi.States.TestBot;

namespace BotTemplateWebApi.Resources;

public partial class BotResources
{
    public string Hello { get; set; }
    public CommonResources Common { get; set; }

    public TestResources Test { get; set; }
}