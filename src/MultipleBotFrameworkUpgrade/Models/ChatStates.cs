using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultipleBotFrameworkUpgrade.Enums;

namespace MultipleBotFrameworkUpgrade.Models;

/// <summary>
/// Класс работы с состояниями чата.
/// </summary>
public class ChatStates
{
    private List<string> _states;

    public ChatStates(List<string> states)
    {
        _states = states;
    }

    public string CurrentState
    {
        get { return _states.Last(); }
        set
        {
            if (_states == null)
            {
                _states = new List<string>();
            }

            if (_states.Any() == false)
            {
                _states.Add(value);
            }
            else
            {
                _states.RemoveAt(_states.Count - 1);
                _states.Add(value);
            }
        }
    }

    public string? PreviousState => _states.Count > 1 ? _states[_states.Count - 2] : null;

    public string RootState => _states.First();

    public List<string> All => _states;

    /// <summary>
    /// Установить состояние чата.
    /// </summary>
    /// <param name="stateName"></param>
    /// <param name="setterType"></param>
    public void Set(string stateName, ChatStateSetterType setterType = ChatStateSetterType.ChangeCurrent)
    {
        switch (setterType)
        {
            case ChatStateSetterType.SetNext:
                _states.Add(stateName);
                return;
            case ChatStateSetterType.ChangeCurrent:
                CurrentState = stateName;
                return;
            case ChatStateSetterType.SetRoot:
                _states.RemoveAll(i => true);
                _states.Add(stateName);
                return;
            default: throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Возвращение в предыдущие состояния.
    /// </summary>
    /// <param name="goBackType"></param>
    public void GoBack(ChatStateGoBackType goBackType)
    {
        switch (goBackType)
        {
            case ChatStateGoBackType.GoToPrevious:
                _states.RemoveAt(_states.Count - 1);
                return;
            case ChatStateGoBackType.GoToRoot:
                string rootState = RootState;
                _states.RemoveAll(i => true);
                _states.Add(rootState);
                return;
            default: throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Переопределяем метод конвертации в строку.
    /// Отображаем состояния чата.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        foreach (string state in _states)
        {
            sb.Append($"\\{state}");
        }

        return sb.ToString();
    }
}