using System;
using System.Threading.Tasks;
using MultipleBotFramework.Base;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Extensions;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.SpecificTypes;

[BotHandler(updateTypes:new [] { UpdateType.Message }, version: 0)]
public class DefaultMessageHandler : BaseBotHandler
{
    public DefaultMessageHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        
    }

    public override async Task HandleBotRequest(Update update)
    {
#if DEBUG
        var m = update.Message;
        
        if (Chat is not null && Chat.ChatId != default)
        {
            try
            {
                if (m.MessageThreadId is not null && m.IsTopicMessage == true)
                { 
                    // Написали в супергруппу (тему topic), нужно ответить внутри topic
                    await BotClient.SendMessageAsync(Chat.ChatId, $"DEFAULT TOPIC [{update.Type().ToString()}]", messageThreadId:m.MessageThreadId);
                }
                else if(m.MessageThreadId is not null && m.IsTopicMessage == null && m.ReplyToMessage?.SenderChat?.Type == "channel")
                {
                    // Написали комментарий в пост канала, нужно ответить.
                    ReplyParameters replyParams = new() { MessageId = m.MessageId };
                    await BotClient.SendMessageAsync(Chat.ChatId, $"DEFAULT COMMENT POST [{update.Type().ToString()}]", replyParameters: replyParams);
                }
                else
                {
                    await BotClient.SendMessageAsync(Chat.ChatId, $"DEFAULT [{update.Type().ToString()}]");
                }
            }
            catch (Exception e)
            {
                
            }
        }
#endif
        return;
    }
}