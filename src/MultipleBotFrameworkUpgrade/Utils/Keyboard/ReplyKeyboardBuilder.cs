using System.Collections.Generic;
using System.Linq;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFrameworkUpgrade.Utils.Keyboard;

public class ReplyKeyboardBuilder : ReplyKeyboardMarkup
{
    private List<List<KeyboardButton>> _keyboard = new();
    private List<KeyboardButton> LastRow
    {
        get
        {
            if (_keyboard is null) _keyboard = new List<List<KeyboardButton>>();
            if (_keyboard.Count() == 0) _keyboard.Add(new List<KeyboardButton>());
            return _keyboard.Last();
        }
        set
        {
            var last = _keyboard.Last();
            last = value;
        }
    }
    
    public ReplyKeyboardBuilder() : base(new List<List<KeyboardButton>>())
    {
        this.ResizeKeyboard = true;
    }

    public ReplyKeyboardBuilder NewRow()
    {
        if (LastRow.Count() == 0) return this;

        _keyboard.Add(new List<KeyboardButton>());

        return this;
    }

    public ReplyKeyboardBuilder Add(KeyboardButton btn)
    {
        LastRow.Add(btn);
        return this;
    }
    
    public ReplyKeyboardBuilder Add(string text)
    {
        return Add(new KeyboardButton(text));
    }

    public ReplyKeyboardBuilder Build()
    {
        this.Keyboard = _keyboard;
        return this;
    }
}