using System;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Attributes;

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