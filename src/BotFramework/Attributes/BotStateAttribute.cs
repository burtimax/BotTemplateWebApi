using System;

namespace BotFramework.Attributes;

public class BotStateAttribute : Attribute
{
    public string StateName { get; set; }
    public double Version { get; set; }

    public BotStateAttribute(string stateName, double version = 0.0D)
    {
        this.StateName = stateName;
        this.Version = version;
    }
}