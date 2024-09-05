using System.Collections.Generic;
using System.Linq;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Utils.Keyboard;

public class InlineKeyboardBuilder : InlineKeyboardMarkup
{
    private List<List<InlineKeyboardButton>> _keyboard = new();
    
    private List<InlineKeyboardButton> LastRow
    {
        get
        {
            if (_keyboard is null) _keyboard = new List<List<InlineKeyboardButton>>();
            if (_keyboard.Count() == 0) _keyboard.Add(new List<InlineKeyboardButton>());
            return _keyboard.Last();
        }
        set
        {
            var last = _keyboard.Last();
            last = value;
        }
    }
    
    public InlineKeyboardBuilder() : base(new List<List<InlineKeyboardButton>>())
    {
    }

    public InlineKeyboardBuilder NewRow()
    {
        if (LastRow.Count() == 0) return this;

       _keyboard.Add(new List<InlineKeyboardButton>());

        return this;
    }

    public InlineKeyboardBuilder Add(InlineKeyboardButton btn)
    {
        LastRow.Add(btn);
        return this;
    }
    
    public InlineKeyboardBuilder Add(string text, string callbackData)
    {
        return Add(new InlineKeyboardButton(text)
        {
            CallbackData = callbackData,
        });
    }

    public InlineKeyboardMarkup Build()
    {
        this.InlineKeyboard = _keyboard;
        return this;
    }
}