using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Utils;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Middleware;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
 
    public LocalizationMiddleware(RequestDelegate next)
    {
        this._next = next;
    }
 
    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments(BotWebhook.WebhookRoute) == false
            || context.Request.Method != "POST")
        {
            await _next(context);
            return;
        }
        
        // Только для Urdate запросов Telegram
        string postData = null;
        using (var reader = new StreamReader(context.Request.Body))
        {
            postData = await reader.ReadToEndAsync();
            Update? update = JsonSerializer.Deserialize<Update>(postData);
            if (update != null)
            {
                User? user = update.GetUser();
                if (user != null)
                {
                    string? lang = user.LanguageCode;
                    //context.Request.QueryString = new QueryString($"?culture={lang}");
                    // Добавляем заголовок, RequestCultureProvider подхватит нужную локализацию в запрос.
                    context.Request.Headers.Add("Accept-Language", lang);
                }
            }
        }
        var requestData = Encoding.UTF8.GetBytes(postData);
        context.Request.Body = new MemoryStream(requestData);
        context.Request.ContentLength = context.Request.Body.Length;
        
        await _next.Invoke(context);
        
    }
}