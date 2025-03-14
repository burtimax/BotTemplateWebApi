﻿using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace MultipleBotFramework_Obsolete.Options;

public class BotHandlerDelegates
{
    /// <summary>
    /// Делегат для обработки Poll запроса бота.
    /// </summary>
    public delegate Task BotPollHandler(Poll poll);
    
    /// <summary>
    /// Делегат для обработки удаления бота у пользователя.
    /// </summary>
    public delegate Task BotDeleteHandler(Update update);
}