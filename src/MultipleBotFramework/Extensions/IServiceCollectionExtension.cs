﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFramework.Db;
using MultipleBotFramework.Dispatcher;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;

namespace MultipleBotFramework.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddBot(this IServiceCollection services, BotConfiguration botConfiguration,
        IEnumerable<ClaimValue>? claims = null, BotOptions botOptions = null)
    {
        // Регистрируем контекст
        AddMultipleBotDb(services, botConfiguration.DbConnection);
        
        // Делаем миграцию в БД.
        var ob = new DbContextOptionsBuilder<BotDbContext>();
        ob.UseNpgsql(botConfiguration.DbConnection);
        
        using (BotDbContext botDbContext = new BotDbContext(ob.Options))
        {
            botDbContext.Database.Migrate();
            
            IEnumerable<ClaimValue> baseClaims = BotConstants.BaseBotClaims.GetBaseBotClaims();
            
            DatabaseBootstrapper.InitializeClaims(botDbContext, 
                    (claims == null || claims.Any() == false) ? baseClaims : baseClaims.Concat(claims))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            var botsManager = new BotsManagerService(botDbContext, botOptions, botConfiguration);
            botsManager.InitializeBotsIfNeed().GetAwaiter().GetResult();
        }
        
        // Регистрируем сервисы.
        services.AddTransient<IBotFactory, BotFactory>();
        services.AddTransient<IBotsManagerService, BotsManagerService>();
        services.AddTransient<BotUpdateDispatcher>();
        AddMultipleBotServices(services);
        
        return services;
    }
    
    public static IServiceCollection AddMultipleBotServices(this IServiceCollection services)
    {
        // Регистрируем сервисы.
        services.AddTransient<IBaseBotRepository, BaseBotRepository>();
        services.AddTransient<IBotUpdateRepository, BotUpdateRepository>();
        services.AddTransient<SaveUpdateService>();
        services.AddTransient<ISavedMessageService, SavedMessageService>();
        services.AddTransient<BotChatHistoryService>();
        services.AddHttpContextAccessor();
        
        return services;
    }
    
    public static IServiceCollection AddMultipleBotDb(this IServiceCollection services, string dbConnection)
    {
        /// Регистрируем контекст
        services.AddDbContext<BotDbContext>(options =>
        {
            options.UseNpgsql(dbConnection);
        });
        return services;
    }
}