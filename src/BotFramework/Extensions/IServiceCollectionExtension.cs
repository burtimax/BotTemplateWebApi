using BotFramework.Options;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BotFramework.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddBot (this IServiceCollection services, BotConfiguration botConfiguration)
    {
        TelegramBotClient botClient = new(botConfiguration.TelegramToken);
        botClient.SetWebhookAsync(botConfiguration.Webhook).Wait();

        services.AddSingleton<ITelegramBotClient>(botClient);

        return services;
    }
}