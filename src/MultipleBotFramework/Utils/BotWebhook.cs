namespace MultipleBotFramework.Utils;

public class BotWebhook
{
      
    /// <summary>
    /// 0 - Адрес.
    /// 1 - ИД бота. 
    /// </summary>
    public const string WebhookFormat = $"{{0}}{WebhookRoute}/{{1}}";
    
    
    /// <summary>
    /// 0 - Наименование параметра для передачи идентификатора.
    /// </summary>
    public const string WebhookRouteFormat = WebhookRoute + "/{0}";
    private const string WebhookRoute = "/bot-update";
    public static string BaseAddress;

    /// <summary>
    /// Сформировать строку webhookа для бота.
    /// </summary>
    /// <param name="botId"></param>
    /// <returns></returns>
    public static string GetWebhookForBot(long botId)
    {
        return string.Format(WebhookFormat, BaseAddress.Trim(' ', '/'), botId);
    }
}