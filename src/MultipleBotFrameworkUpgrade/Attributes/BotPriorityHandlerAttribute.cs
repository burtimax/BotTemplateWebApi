using System;
using MultipleBotFrameworkUpgrade.Enums;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BotPriorityHandlerAttribute : Attribute
{
    public UpdateType UpdateType { get; set; }
    public double Version { get; set; }
    public string? UserRole { get; set; }

    public BotPriorityHandlerAttribute(UpdateType updateType, double version = 0.0D, string userRole = null)
    {
        this.UpdateType = updateType;
        this.Version = version;
        this.UserRole = userRole;
    }
}