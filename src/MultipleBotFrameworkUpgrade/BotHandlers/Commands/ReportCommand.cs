using System;
using System.Threading.Tasks;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Constants;
using MultipleBotFrameworkUpgrade.Models;
using MultipleBotFrameworkUpgrade.Utils.ReportGenerator;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFrameworkUpgrade.BotHandlers.Commands;

/// <summary>
/// Команда для получения отчета по работе бота за последние N часов.
/// /report {number_hours}
/// </summary>
[BotCommand(Name, version: 1, 
    RequiredUserClaims = new[] { BotConstants.BaseBotClaims.BotReportGet })]
public class ReportCommand : BaseBotCommand
{
    public const string Name = "/report";

    private const int _DefaultHoursReport = 24;
    
    public ReportCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        string[] words = update.Message.Text.Split(' ');
        if (words.Length < 1)
        {
            await BotClient.SendMessageAsync(Chat.ChatId,
                "Команда не правильна, добавьте параметр (кол-во часов) через пробел.\nНапример: [/report 24]");
            return;
        }

        int hours = _DefaultHoursReport;
        
        if (words.Length > 1 && string.IsNullOrEmpty(words[1]) == false && int.TryParse(words[1], out hours) == false)
        {
            await BotClient.SendMessageAsync(Chat.ChatId,
                "Введите числовое значение после команды (кол-во часов).\nНапример: [/report 24]");
            return;
        }

        BotActivityReportGenerator reportGenerator = new();
        string report = await reportGenerator.GetReport(BotDbContext, DateTime.Now.AddHours(-hours), DateTime.Now);
        await BotClient.SendMessageAsync(Chat.ChatId, report, parseMode:ParseMode.Html);
    }
}