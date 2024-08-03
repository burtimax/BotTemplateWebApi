using System.Threading.Tasks;
using Telegram.Bot;

namespace MultipleBotFramework.Services;

public interface IBotFactory
{
    ITelegramBotClient? GetInstance(long botId);
}