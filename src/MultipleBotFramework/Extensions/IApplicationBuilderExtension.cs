using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Builder;
using MultipleBotFramework.Middleware;

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
        return builder;
    }
}