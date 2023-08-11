using System;

namespace BotFramework.Attributes;

public class BotCommandAttribute : Attribute
{
    public string Command { get; set; }
    public double Version { get; set; }
    public string UserRole { get; set; }

    public BotCommandAttribute(string command, string userRole = null, double version = 0.0D)
    {
        this.Command = command;
        UserRole = userRole;
        this.Version = version;
    }
}