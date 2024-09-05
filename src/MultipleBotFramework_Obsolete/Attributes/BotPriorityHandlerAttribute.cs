using System;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework_Obsolete.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BotPriorityHandlerAttribute : Attribute
{
    public UpdateType UpdateType { get; set; }
    public double Version { get; set; }

    public BotPriorityHandlerAttribute(UpdateType updateType, double version = 0.0D)
    {
        this.UpdateType = updateType;
        this.Version = version;
    }
}