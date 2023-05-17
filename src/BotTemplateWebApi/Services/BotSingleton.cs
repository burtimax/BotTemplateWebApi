using System.Diagnostics;
using BotFramework.Base;
using BotTemplateWebApi.App.Options;
using BotTemplateWebApi.Interfaces.IServices;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace BotTemplateWebApi.Services
{
    public class BotSingleton : IBotSingleton
    {
        public static Bot Bot;
        private readonly string _token;
        private readonly string _webhook;
        

        public BotSingleton(string token, string webhook)
        {
            _token = token;
            _webhook = webhook;
            var bot = GetInstance();
            Debug.WriteLine("BOT URL - " + bot.ApiClient.GetWebhookInfoAsync().Result.Url);
            Debug.WriteLine("BOT IP - " + bot.ApiClient.GetWebhookInfoAsync().Result.IpAddress);
            Debug.WriteLine("BOT PENDING COUNT - " + bot.ApiClient.GetWebhookInfoAsync().Result.PendingUpdateCount.ToString());
        }
        
        public BotSingleton(IOptions<ApplicationConfiguration.BotConfiguration> botConfig)
        : this(botConfig.Value.TelegramToken, botConfig.Value.Webhook)
        {
        }
        
        public Bot GetInstance()
        {
            if (Bot != null) return Bot;
            
            TelegramBotClient botClient = new(_token);
            botClient.SetWebhookAsync(_webhook).Wait();
            Bot = new Bot(botClient);
            return Bot;
        }
    }
}