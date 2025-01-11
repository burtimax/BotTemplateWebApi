using System.Threading.Tasks;
using MultipleBotFramework.Models;
using Telegram.BotAPI;

namespace MultipleBotFramework.Base;

public class Delegates
{
    public delegate Task BotNotificationAction(ITelegramBotClient botClient, long chatId);
}