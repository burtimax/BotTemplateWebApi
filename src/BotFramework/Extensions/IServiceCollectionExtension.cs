using BotFramework.Db;
using BotFramework.Implementation;
using BotFramework.Interfaces;
using BotFramework.Options;
using BotFramework.Repository;
using Microsoft.EntityFrameworkCore;
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
        
        services.AddDbContext<BotDbContext>(options =>
        {
            options.UseNpgsql(botConfiguration.DbConnection);
        });
        
        services.AddTransient<IBaseBotRepository, BaseBotRepository>();
        services.AddTransient<IBotUpdateRepository, BotUpdateRepository>();
        services.AddTransient<ISaveUpdateService, SaveUpdateService>();

        return services;
    }
}