using Telegram.BotAPI;

namespace MultipleBotFrameworkUpgrade.Services;

public interface IBotFactory
{
    ITelegramBotClient? GetInstance(long botId);
}