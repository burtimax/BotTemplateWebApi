using System;
using System.Linq;

namespace MultipleBotFrameworkUpgrade.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class BotCommandAttribute : Attribute
{
    public string Command { get; set; }
    public double Version { get; set; }
    public string? RequiredUserRole { get; set; }
    
    public string[] RequiredUserClaims { get; set; }
    
    public BotCommandAttribute(string command, 
        string requiredUserRole = null,
        string[] requiredUserClaims = null,
        double version = 0.0D)
    {
        this.Command = command;
        this.RequiredUserRole = requiredUserRole;
        this.RequiredUserClaims = requiredUserClaims != null && requiredUserClaims.Any() ? requiredUserClaims : null;
        this.Version = version;
    }
}