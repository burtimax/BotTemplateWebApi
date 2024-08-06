using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Dto;
using MultipleBotFrameworkUpgrade.Options;
using MultipleBotFrameworkUpgrade.Repository;
using MultipleBotFrameworkUpgrade.Services;
using MultipleBotFrameworkUpgrade.Utils;

namespace MultipleBotFrameworkUpgrade.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddBot(this IServiceCollection services, BotConfiguration botConfiguration,
        IEnumerable<ClaimValue>? claims = null, BotOptions botOptions = null)
    {
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
            
            DatabaseBootstrapper.InitializeClaims(botDbContext, 
                    (claims == null || claims.Any() == false) ? baseClaims : baseClaims.Concat(claims))
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

            BotFactory botFactory = new BotFactory(botDbContext, botConfiguration, botOptions);
            services.AddSingleton<IBotFactory>(botFactory);
        }
        
        // Регистрируем сервисы.
        services.AddTransient<IBaseBotRepository, BaseBotRepository>();
        services.AddTransient<IBotUpdateRepository, BotUpdateRepository>();
        services.AddTransient<ISaveUpdateService, SaveUpdateService>();
        services.AddTransient<ISavedMessageService, SavedMessageService>();

        return services;
    }
}