using BotFramework.Base;
using BotTemplateWebApi.App.Options;
using BotTemplateWebApi.Interfaces.IServices;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace BotTemplateWebApi.Services
{
    public class BotSingleton : IBotSingleton
    {
        private Bot _bot;
        private readonly ApplicationConfiguration.BotConfiguration _botConfig;

        public BotSingleton(IOptions<ApplicationConfiguration.BotConfiguration> options, 
            IOptions<ApplicationConfiguration.BotConfiguration> botConfig)
        {
            _botConfig = botConfig.Value;
        }
        
        public async ValueTask<Bot> GetInstance()
        {
            if (_bot != null) return _bot;
            
            TelegramBotClient botClient = new(_botConfig.TelegramToken);
            await botClient.SetWebhookAsync(_botConfig.Webhook);
            _bot = new Bot(botClient);
            return _bot;
        }
    }
}