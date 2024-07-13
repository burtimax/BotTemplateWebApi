using BotFramework.Attributes;
using BotFramework.Base;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.BotHandlers.UpdateType;

[BotPriorityHandler(Telegram.Bot.Types.Enums.UpdateType.PollAnswer)]
public class MessageTypeHandler : BaseBotPriorityHandler
{
    public MessageTypeHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        PollAnswer answer = update.PollAnswer;
        await BotClient.SendTextMessageAsync(Chat.ChatId, string.Join(',', answer.OptionIds));
    }
}