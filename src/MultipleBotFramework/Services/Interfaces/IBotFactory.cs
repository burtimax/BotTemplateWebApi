using System.Threading.Tasks;
using Telegram.BotAPI;

namespace MultipleBotFramework.Services.Interfaces;

public interface IBotFactory
{
    Task<ITelegramBotClient?> GetInstance(long botId);
}