using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Db.BroadcastDb;
using MultipleBotFramework.Middleware;
using MultipleBotFramework.Options;

namespace MultipleBotFramework.Extensions;

public static class IApplicationBuilderExtension
{
    public static IApplicationBuilder UseBot(this IApplicationBuilder builder, CultureInfo[] supportedCultures = null)
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
            DefaultRequestCulture = new RequestCulture("en"),
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
        
        return builder;
    }
    
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
}