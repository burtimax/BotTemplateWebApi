using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using BotFramework.Db;
using BotFramework.Dto;
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
        IEnumerable<ClaimValue>? claims = null, Action<BotOptions> botOptions = null)
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
            
            IEnumerable<ClaimValue> baseClaims = BotConstants.BaseBotClaims.GetBaseBotClaims();
            
            DatabaseBootstrapper.InitializeClaimsIfNeed(botDbContext, 
                    (claims == null || claims.Any() == false) ? baseClaims : baseClaims.Concat(claims))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();
        }
        
        // Регистрируем сервисы.
        services.AddTransient<IBaseBotRepository, BaseBotRepository>();
        services.AddTransient<IBotUpdateRepository, BotUpdateRepository>();
        services.AddTransient<ISaveUpdateService, SaveUpdateService>();

        return services;
    }
}