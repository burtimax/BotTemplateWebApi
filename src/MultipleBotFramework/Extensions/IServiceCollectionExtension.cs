using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Dispatcher;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Options;
using MultipleBotFramework.Quartz.Jobs;
using MultipleBotFramework.Quartz.Jobs.BroadcastNotification;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;
using Quartz;

namespace MultipleBotFramework.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddBot(this IServiceCollection services, IConfiguration configuration,
        IEnumerable<ClaimValue>? claims = null)
    {
        var botConf = configuration.GetSection(BotConfiguration.Section);
        if(!botConf.Exists()) throw new NullReferenceException($"Missing configuration {BotConfiguration.Section}");
        var botConfiguration = configuration.GetSection(BotConfiguration.Section).Get<BotConfiguration>();
        services.AddSingleton<BotConfiguration>(botConfiguration);
        
        var botOptConf = configuration.GetSection(BotOptions.Section);
        if(!botOptConf.Exists()) throw new NullReferenceException($"Missing configuration {BotOptions.Section}");
        var botOptions = configuration.GetSection(BotOptions.Section).Get<BotOptions>();
        services.AddSingleton<BotOptions>(botOptions);
        
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
        services.AddMultipleBotServices();
        services.AddBroadcast(configuration);
        return services;
    }
    
    public static IServiceCollection AddMultipleBotServices(this IServiceCollection services)
    {
        // Регистрируем сервисы.
        services.AddTransient<IBaseBotRepository, BaseBotRepository>();
        services.AddTransient<IBotUpdateRepository, BotUpdateRepository>();
        services.AddTransient<SaveUpdateService>();
        services.AddTransient<ISavedMessageService, SavedMessageService>();
        services.AddTransient<IBotNotificationService, BotNotificationService>();
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
    
    public static void AddBroadcast(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(BroadcastConfiguration.Section).Get<BroadcastConfiguration>();

        if (settings == default || settings.IsEnabled == false) return;

        services.AddSingleton<BroadcastConfiguration>(settings);

        services.AddDbContext<BroadcastDbContext>(options =>
        {
            options.UseNpgsql(settings.BroadcastDbConnection);
        });
        
        services.AddTransient<IBroadcastTaskService, BroadcastTaskService>();

        AddQuartzBroadcast(services);
    }
    
    private static void AddQuartzBroadcast(IServiceCollection services)
    {
        services.AddQuartz(quartzConfigurator =>
        {
            quartzConfigurator.UseMicrosoftDependencyInjectionJobFactory();
            quartzConfigurator.AddJob<BroadcastNotificationJob>(jobConfigurator =>
            {
                jobConfigurator.WithIdentity(BroadcastNotificationJob.Key);
            });
            quartzConfigurator.AddTrigger(triggerConfigurator =>
            {
                triggerConfigurator.ForJob(BroadcastNotificationJob.Key)
                    .WithIdentity(new TriggerKey("bot-notification-job", "bot-triggers"))
                    .WithSimpleSchedule(x => x.WithInterval(TimeSpan.FromSeconds(10)).RepeatForever())
                    .StartNow();
            });
        });
        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
    }
}