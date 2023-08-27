using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BotFramework.Other;

/// <summary>
/// Валидирует правильность json строки ресурсов бота.
/// </summary>
public class BotResourcesValidator
{
    public static Regex SubstitutePattern = new Regex(@"{{(?<key>[a-z, A-Z, 0-9, \._]*)(\s*|(\#(?<callback>\w*)))}}",
        RegexOptions.Multiline | RegexOptions.IgnoreCase);
    
    /// <summary>
    /// Валидирует правильность json строки ресурсов бота.
    /// </summary>
    public bool IsResourcesDataValid(string resourcesJson, out List<string>? exceptions)
    {
        exceptions = new List<string>();
        JObject jrs = JObject.Parse(resourcesJson);

        MatchCollection matches = SubstitutePattern.Matches(resourcesJson);
        
        if (matches.Any() == false) return true;

        foreach (Match match in matches)
        {
            JToken? value = GetValueByPath(jrs, match.Groups["key"].Value);
            if(value is null) exceptions.Add($"Not found property [{match.Value}] on position [{match.Index}].");
        }

        if (exceptions.Any() == false)
        {
            exceptions = null;
            return true;
        }

        return false;
    }

    private JToken? GetValueByPath(JObject jObject, string keyPath)
    {
        JToken? found = jObject.Root.SelectToken(keyPath);
        return found;
    }
}