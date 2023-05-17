using System.Collections.Generic;
using System.Linq;

namespace BotFramework.Models;

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
        get
        {
            return _states.Last();
        }
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

    public string FirstState => _states.First();

    public List<string> All => _states;

}