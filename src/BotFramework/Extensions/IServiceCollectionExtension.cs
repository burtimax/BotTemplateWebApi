using System;
using System.Configuration;
using System.IO;
using BotFramework.Db;
using BotFramework.Options;
using BotFramework.Other;
using BotFramework.Repository;
using BotFramework.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BotFramework.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddBot(this IServiceCollection services, BotConfiguration botConfiguration,
        Action<BotOptions> botOptions = null)
    {
        TelegramBotClient botClient = new(botConfiguration.TelegramToken);
        botClient.SetWebhookAsync(botConfiguration.Webhook).Wait();
        services.AddSingleton<ITelegramBotClient>(botClient);

        // Регистрируем контекст
        services.AddDbContext<BotDbContext>(options =>
        {
            options.UseNpgsql(botConfiguration.DbConnection);
        });
        
        // Делаем миграцию в БД.
        var ob = new DbContextOptionsBuilder<BotDbContext>();
        ob.UseNpgsql(botConfiguration.DbConnection);

        using (BotDbContext botDbContext = new BotDbContext(ob.Options))
        {
            botDbContext.Database.Migrate();
        }
        
        // Регистрируем сервисы.
        services.AddTransient<IBaseBotRepository, BaseBotRepository>();
        services.AddTransient<IBotUpdateRepository, BotUpdateRepository>();
        services.AddTransient<ISaveUpdateService, SaveUpdateService>();

        return services;
    }
}