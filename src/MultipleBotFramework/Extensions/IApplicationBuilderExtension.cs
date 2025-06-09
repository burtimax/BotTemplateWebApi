using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Db.ReferralDb;
using MultipleBotFramework.Middleware;
using MultipleBotFramework.Options;

namespace MultipleBotFramework.Extensions;

/// <summary>
/// Расширения для IApplicationBuilder, добавляющие функциональность бота.
/// </summary>
public static class IApplicationBuilderExtension
{
    /// <summary>
    /// Настраивает приложение для работы с ботом, включая локализацию, рассылки и реферальную программу.
    /// </summary>
    /// <param name="builder">Построитель приложения</param>
    /// <param name="defaultCulture">Культура по умолчанию</param>
    /// <param name="supportedCultures">Поддерживаемые культуры</param>
    /// <returns>Построитель приложения</returns>
    public static IApplicationBuilder UseBot(this IApplicationBuilder builder, CultureInfo defaultCulture = null, CultureInfo[] supportedCultures = null)
    {
        builder.UseMiddleware<LocalizationMiddleware>();
        
        if (supportedCultures == null || supportedCultures.Length == 0)
        {
            supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("en-GB"),
                new CultureInfo("en"),
                new CultureInfo("ru-RU"),
                new CultureInfo("ru"),
            };
        }
        
        builder.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(defaultCulture?.Name ?? "en"),
            SupportedCultures = supportedCultures,
            SupportedUICultures = supportedCultures,
            RequestCultureProviders = new List<IRequestCultureProvider>()
            {
                // new QueryStringRequestCultureProvider(), // Отключаем пока
                // new CookieRequestCultureProvider(), // Отключаем пока
                new AcceptLanguageHeaderRequestCultureProvider()
            },
        });
        
        UseBroadcast(builder);
        UseReferral(builder);
        
        return builder;
    }
    
    /// <summary>
    /// Настраивает приложение для работы с рассылками.
    /// </summary>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Построитель приложения</returns>
    public static IApplicationBuilder UseBroadcast(this IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var config = scope.ServiceProvider.GetService<BroadcastConfiguration>();
            if (config == null || config.IsEnabled == false) return builder;
            
            var context = scope.ServiceProvider.GetRequiredService<BroadcastDbContext>();
            context.Database.Migrate();
        }

        return builder;
    }
    
    /// <summary>
    /// Настраивает приложение для работы с реферальной программой.
    /// </summary>
    /// <param name="builder">Построитель приложения</param>
    /// <returns>Построитель приложения</returns>
    public static IApplicationBuilder UseReferral(this IApplicationBuilder builder)
    {
        using (var scope = builder.ApplicationServices.CreateScope())
        {
            var config = scope.ServiceProvider.GetService<BotReferralConfiguration>();
            if (config == null || config.IsEnabled == false) return builder;
            
            var context = scope.ServiceProvider.GetRequiredService<ReferralDbContext>();
            context.Database.Migrate();
        }

        return builder;
    }
}