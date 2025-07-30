using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Db.ReferralDb;
using MultipleBotFramework.Dispatcher;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Options;
using MultipleBotFramework.Quartz.Jobs;
using MultipleBotFramework.Quartz.Jobs.BroadcastNotification;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Services.Referral;
using MultipleBotFramework.Utils;
using Quartz;

namespace MultipleBotFramework.Extensions;

/// <summary>
/// Расширения для IServiceCollection, добавляющие сервисы бота.
/// </summary>
public static class IServiceCollectionExtension
{
    /// <summary>
    /// Добавляет сервисы бота в коллекцию сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    /// <param name="claims">Дополнительные утверждения</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddBot(this IServiceCollection services, IConfiguration configuration,
        IEnumerable<ClaimValue>? claims = null,
        IEnumerable<string>? allowedUpdates = null)
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

            if (allowedUpdates is not null && allowedUpdates?.Any() == true)
            {
                BotsManagerService.AllowedUpdates = allowedUpdates;
            }
            
            var botsManager = new BotsManagerService(botDbContext, botOptions, botConfiguration);
            botsManager.InitializeBotsIfNeed().GetAwaiter().GetResult();
        }
        
        // Регистрируем сервисы.
        services.AddTransient<IBotFactory, BotFactory>();
        services.AddTransient<IBotsManagerService, BotsManagerService>();
        services.AddTransient<BotUpdateDispatcher>();
        services.AddMultipleBotServices();
        services.AddBroadcast(configuration);
        services.AddReferrals(configuration);
        return services;
    }
    
    /// <summary>
    /// Добавляет сервисы для работы с несколькими ботами.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns>Коллекция сервисов</returns>
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
    
    /// <summary>
    /// Добавляет контекст базы данных для нескольких ботов.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="dbConnection">Строка подключения к базе данных</param>
    /// <returns>Коллекция сервисов</returns>
    public static IServiceCollection AddMultipleBotDb(this IServiceCollection services, string dbConnection)
    {
        /// Регистрируем контекст
        services.AddDbContext<BotDbContext>(options =>
        {
            options.UseNpgsql(dbConnection);
        });
        return services;
    }
    
    /// <summary>
    /// Добавляет сервисы для работы с рассылками.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    public static void AddBroadcast(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(BroadcastConfiguration.Section).Get<BroadcastConfiguration>();

        if (settings == default || settings.IsEnabled == false) return;

        string dbConnection = configuration.GetSection(BotConfiguration.Section).Get<BotConfiguration>()!.DbConnection;
        services.AddSingleton<BroadcastConfiguration>(settings);

        services.AddDbContext<BroadcastDbContext>(options =>
        {
            options.UseNpgsql(dbConnection);
        });
        
        services.AddTransient<IBroadcastTaskService, BroadcastTaskService>();

        AddQuartzBroadcast(services);
    }
    
    /// <summary>
    /// Добавляет сервисы для работы с реферальной программой.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
    /// <param name="configuration">Конфигурация приложения</param>
    public static void AddReferrals(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(BotReferralConfiguration.Section).Get<BotReferralConfiguration>();

        if (settings == default || settings.IsEnabled == false) return;

        string dbConnection = configuration.GetSection(BotConfiguration.Section).Get<BotConfiguration>()!.DbConnection;
        services.AddSingleton<BotReferralConfiguration>(settings);

        services.AddDbContext<ReferralDbContext>(options =>
        {
            options.UseNpgsql(dbConnection);
        });
        
        services.AddTransient<IReferralService, ReferralService>();
        services.AddTransient<IReferralCodeService, ReferralCodeService>();
    }
    
    /// <summary>
    /// Добавляет планировщик задач Quartz для рассылок.
    /// </summary>
    /// <param name="services">Коллекция сервисов</param>
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