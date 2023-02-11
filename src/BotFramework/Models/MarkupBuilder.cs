using System.Collections.Generic;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Models
{
    /// <summary>
    /// Строитель клавиатуры
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MarkupBuilder<T> where T : IReplyMarkup
    {
        /// <summary>
        /// Свойство отвечает за автоматическое определение размеров кнопок (клавиатуры пользователя) 
        /// </summary>
        public bool ResizeKeyboard { get; set; } = true;
        
        private IKeyboardButton[][] _keyboardMatrix;
        private readonly List<List<IKeyboardButton>> _keyboard;

        public MarkupBuilder(bool resizeKeyboard = true) : this()
        {
            this.ResizeKeyboard = resizeKeyboard;
        }

        public MarkupBuilder()
        {
            this._keyboard = new List<List<IKeyboardButton>>();
        }

        List<IKeyboardButton> LastRow
        {
            get
            {
                if(this._keyboard.Count>0) return this._keyboard.LastOrDefault();
                return null;
            }
        }

        public MarkupBuilder<T> NewRow()
        {
            this._keyboard.Add(new List<IKeyboardButton>());
            return this;
        }

        /// <summary>
        /// Добавляем кнопку в клавиатуру
        /// </summary>
        /// <param name="text">Имя кнопки</param>
        /// <param name="callBack">Используется только для InlineKeyboardMarkup</param>
        /// <returns></returns>
        public MarkupBuilder<T> Add(string text, string callBack = "default", string url = null)
        {

            if (typeof(T) == typeof(ReplyKeyboardMarkup)){
                KeyboardButton btn = new KeyboardButton(text);
                LastRow.Add(btn);
            }

            if (typeof(T) == typeof(InlineKeyboardMarkup))
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    InlineKeyboardButton btn = new InlineKeyboardButton(text);
                    btn.CallbackData = callBack;
                    LastRow.Add(btn);
                }
                else
                {
                    InlineKeyboardButton btn = InlineKeyboardButton.WithUrl(text, url);
                    LastRow.Add(btn);
                }
            }

            return this;
        }

        public IReplyMarkup Build()
        {
            IReplyMarkup replyMarkup = null;
            
            if (typeof(T) == typeof(InlineKeyboardMarkup))
            {
                this._keyboardMatrix = new InlineKeyboardButton[_keyboard.Count][];
                for (int i = 0; i < this._keyboardMatrix.Length; i++)
                {
                    this._keyboardMatrix[i] = new InlineKeyboardButton[this._keyboard[i].Count];
                }

                for (int i = 0; i < _keyboard.Count; i++)
                {
                    for (int j = 0; j < _keyboard[i].Count; j++)
                    {
                        this._keyboardMatrix[i][j] = this._keyboard[i][j];
                    }
                }
            }


            if (typeof(T) == typeof(ReplyKeyboardMarkup))
            {
                this._keyboardMatrix = new KeyboardButton[_keyboard.Count][];
                for (int i = 0; i < this._keyboardMatrix.Length; i++)
                {
                    this._keyboardMatrix[i] = new KeyboardButton[this._keyboard[i].Count];
                }

                for (int i = 0; i < _keyboard.Count; i++)
                {
                    for (int j = 0; j < _keyboard[i].Count; j++)
                    {
                        this._keyboardMatrix[i][j] = this._keyboard[i][j];
                    }
                }
            }

            if (typeof(T) == typeof(InlineKeyboardMarkup))
            {
                replyMarkup = new InlineKeyboardMarkup(this._keyboardMatrix as InlineKeyboardButton[][]);
            }

            if (typeof(T) == typeof(ReplyKeyboardMarkup))
            {
                string[][] btns = new string[_keyboardMatrix.Length][];
                for (int i = 0; i < _keyboardMatrix.Length; i++)
                {
                    btns[i] = new string[_keyboardMatrix[i].Length];
                    for (int j = 0; j < _keyboardMatrix[i].Length; j++)
                    {
                        btns[i][j] = _keyboardMatrix[i][j].Text;
                    }
                }
                
                var tmp = (ReplyKeyboardMarkup)btns;
                tmp.ResizeKeyboard = this.ResizeKeyboard;
                replyMarkup = tmp;
            }

            return replyMarkup;
        }

        /// <summary>
        /// Получить список строк всех кнопок в MarkUpе
        /// </summary>
        public List<string> ButtonCaptions()
        {
            if (Equals(this._keyboard, null) == true ||
                this._keyboard?.Count == 0)
            {
                return null;
            }
            List<string> res = new List<string>();
            for(var i = 0; i < this._keyboard.Count; i++)
            {
                for (var j = 0; j < this._keyboard[i].Count; j++)
                {
                    res.Add(this._keyboard[i][j].Text);
                }
            }

            return res;
        }
    }
}
