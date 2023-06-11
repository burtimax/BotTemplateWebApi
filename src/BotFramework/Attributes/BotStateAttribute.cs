﻿using System;

namespace BotFramework.Attributes;

public class BotStateAttribute : Attribute
{
    public string StateName { get; set; }
    public double Version { get; set; }
    public string UserRole { get; set; }

    public BotStateAttribute(string stateName, string userRole = null, double version = 0.0D)
    {
        this.StateName = stateName;
        this.Version = version;
    }
}