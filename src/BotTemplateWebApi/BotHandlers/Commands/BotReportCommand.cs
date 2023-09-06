using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Other.ReportGenerator;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.BotHandlers.Commands;

[BotCommand("/report", version: 1)]
public class BotReportCommand : BaseBotCommand
{
    public BotReportCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        string[] words = update.Message.Text.Split(' ');
        if (words.Length < 2)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId,
                "Команда не правильна, добавьте параметр (кол-во часов) через пробел.\nНапример: [/report 24]");
            return;
        }

        if (int.TryParse(words[1], out int hours) == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId,
                "Введите числовое значение после команды (кол-во часов).\nНапример: [/report 24]");
            return;
        }

        BotActivityReportGenerator reportGenerator = new();
        string report = await reportGenerator.GetReport(BotDbContext, DateTime.Now.AddHours(-hours), DateTime.Now);
        await BotClient.SendTextMessageAsync(Chat.ChatId, report);
    }
}