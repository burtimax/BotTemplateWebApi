using Telegram.Bot;

namespace MultipleBotFramework_Obsolete.Services;

public interface IBotFactory
{
    ITelegramBotClient? GetInstance(long botId);
}