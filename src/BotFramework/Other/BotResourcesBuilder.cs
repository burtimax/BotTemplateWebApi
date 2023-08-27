using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace BotFramework.Other;

/// <summary>
/// Класс формирует ресурсы для бота.
/// Подставляет строки-переменные.
/// Проверяет валидность json строки ресурсов. 
/// </summary>
public class BotResourcesBuilder
{
    private string _resources;
    protected readonly BotResourcesValidator _validator;
    
    public BotResourcesBuilder(string resourcesJson)
    {
        _resources = resourcesJson;
        _validator = new BotResourcesValidator();
    }

    /// <summary>
    /// Возвращает json ресурсов бота в конечном виде.
    /// </summary>
    /// <returns>Возвращает json после подстановки переменных.</returns>
    /// <exception cref="Exception"></exception>
    public string Build()
    {
        if (_validator.IsResourcesDataValid(_resources, out var exceptions) == false)
        {
            throw new Exception($"Bot recources file has errors.\n" + string.Join("\n", exceptions));
        }

        _resources = SubstituteVariablesInJson(_resources);
        return _resources;
    }

    private string SubstituteVariablesInJson(string json)
    {
        JObject jObj = JObject.Parse(json);

        var match = BotResourcesValidator.SubstitutePattern.Match(json);
        while (match.Success)
        {
            string key = match.Groups["key"].Value;
            JToken val = GetValueByPath(jObj, key)!;
            string value = val.Value<string>();
            json = json.Remove(match.Index, match.Length);
            json = json.Insert(match.Index, value);
            match = BotResourcesValidator.SubstitutePattern.Match(json);
        }

        return json;
    }
    
    private JToken? GetValueByPath(JObject jObject, string keyPath)
    {
        JToken? found = jObject.Root.SelectToken(keyPath);
        return found;
    }
}