using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BotFramework.Exceptions;
using Newtonsoft.Json;

namespace BotFramework.Models;

/// <summary>
/// Cловарь различных типов данных.
/// </summary>
/// <remarks>
/// Реализовано безопасное получение данных из словаря.
/// Реализована возможность хранить объекты любого типа данных.
/// </remarks>
public class ComplexDictionary
{
    private Dictionary<string, string> _data;

    public ComplexDictionary(Dictionary<string, string> data)
    {
        _data = data;
    }
    
    public T Get<T>(string key)
    {
        string value = Get(key);

        T result = JsonConvert.DeserializeObject<T>(value);
        return result;
    }

    public string Get(string key)
    {
        if (_data.TryGetValue(key, out string value) == false)
        {
            throw new UserPropertiesHasNotValueByKeyException(key);
        }

        return JsonConvert.DeserializeObject<string>(value);
    }

    public void Set(string key, object value)
    {
        if (Contains(key))
        {
            Remove(key);
        }

        string addValue = JsonConvert.SerializeObject(value);
        
        _data.Add(key, addValue);
    }

    public void Remove(string key)
    {
        if (Contains(key))
        {
            _data.Remove(key);
        }
    }

    public void RemoveRange(params string[] keys)
    {
        foreach (string key in keys)
        {
            Remove(key);
        }
    }

    public bool Contains(string key)
    {
        return _data.ContainsKey(key);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (KeyValuePair<string,string> keyBValuePair in _data)
        {
            sb.AppendLine($"[{keyBValuePair.Key}] : [{keyBValuePair.Value}]");
        }

        return sb.ToString();
    }
}