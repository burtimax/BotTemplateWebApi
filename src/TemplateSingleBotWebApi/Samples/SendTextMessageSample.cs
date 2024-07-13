using BotFramework.Attributes;
using BotFramework.Base;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.Samples;

/// <summary>
/// Пример обработки текстовых сообщений.
/// Получение и отправка текста пользователю.
/// </summary>
[BotState("SendTextMessageSample", version:1)]
public class SendTextMessageSample : BaseBotState
{
    public SendTextMessageSample(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        string receivedMessage = update.Message.Text;
        await BotClient.SendTextMessageAsync(Chat.ChatId, $"Вы отправили сообщение: {receivedMessage}");
    }
}