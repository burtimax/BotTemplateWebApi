


using Microsoft.Extensions.Options;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Utils;
using MultipleTestBot.Resources;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleTestBot.BotHandlers.Command;

public class BaseMultipleTestBotCommand : BaseBotHandler
{
    protected IServiceProvider ServiceProvider;
    protected readonly BotResources R;

    public BaseMultipleTestBotCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        this.ServiceProvider = serviceProvider;
        R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
    }
    

    protected Task Answer(string text, string parseMode = ParseMode.Html/*, IReplyMarkup replyMarkup = default*/)
    {
        /*if (replyMarkup == default)
        {
            // MarkupBuilder<ReplyKeyboardMarkup> reply = new();
            // reply.NewRow().Add(R.ButtonNext);
            // replyMarkup = reply.Build();
        }*/
        
        return BotClient.SendMessageAsync(Chat.ChatId, text, parseMode: parseMode);
    }
    
    public async Task ChangeState(string stateName, ChatStateSetterType setterType = ChatStateSetterType.ChangeCurrent)
    {
        Chat.States.Set(stateName, setterType);
        await BotDbContext.SaveChangesAsync();
    }

    public override Task HandleBotRequest(Update update)
    {
        throw new NotImplementedException();
    }
    
}