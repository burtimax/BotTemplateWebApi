using Telegram.BotAPI;

namespace MultipleBotFramework.Services;

public interface IBotFactory
{
    ITelegramBotClient? GetInstance(long botId);
}