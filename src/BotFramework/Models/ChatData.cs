﻿using System;
using System.Collections.Generic;
using System.Linq;
using BotFramework.Exceptions;
using Newtonsoft.Json;

namespace BotFramework.Models;

/// <summary>
/// Класс работы с данными чата.
/// </summary>
public class ChatData
{
    private Dictionary<string, string> _data;

    public ChatData(Dictionary<string, string> data)
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
            throw new ChatDataHasNotValueByKeyException(key);
        }

        return value;
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
}