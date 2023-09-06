using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace BotFramework.Other.ReportGenerator;

public class BotActivityReportGenerator
{
    /// <summary>
    /// 0 - Общее кол-во пользователей
    /// 1 - Общее кол-во запросов
    /// 2 - Общее кол-во ошибок
    /// </summary>
    private string botReport = "Данные по боту:\n" +
                               "Кол-во польователей: {0}\n" +
                               "Кол-во запросов: {1}\n" +
                               "Кол-во ошибок: {2}\n";

    /// <summary>
    /// 0 - Дата начала периода
    /// 1 - Дата окончания периода
    /// 2 - Кол-во новых пользователей
    /// 3 - Кол-во активных пользователей
    /// 4 - Кол-во запросов за период
    /// 5 - Кол-во ошибок за период
    /// </summary>
    private string periodBotReport = "{0} - {1}\n" +
                                     "Новые пользователи: {2}\n" +
                                     "Активные пользователи: {3}\n" +
                                     "Кол-во запросов: {4}\n" +
                                     "Кол-во ошибок: {5}\n";
    
    /// <summary>
    /// Формируем отчет по активности бота.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public async Task<string> GetReport(BotDbContext db, DateTime start, DateTime end)
    {
        StringBuilder sb = new();

        int summaryUsersCount = await db.Users.CountAsync();
        int summaryUpdatesCount = await db.Updates.CountAsync();
        int summaryExceptionsCount = await db.Exceptions.CountAsync();

        sb.AppendLine(string.Format(botReport,
            summaryUsersCount,
            summaryUpdatesCount,
            summaryExceptionsCount));

        int periodNewUsers = await db.Users.Where(u => u.CreatedAt >= start && u.CreatedAt <= end).CountAsync();
        int periodActiveUsers = await db.Updates.Where(u => u.CreatedAt >= start && u.CreatedAt <= end)
            .Select(u=>u.BotChatId)
            .Distinct()
            .CountAsync();
        int periodUpdatesCount = await db.Updates.Where(u => u.CreatedAt >= start && u.CreatedAt <= end).CountAsync();
        int periodExceptionsCount = await db.Exceptions.Where(e => e.CreatedAt >= start && e.CreatedAt <= end).CountAsync();

        sb.AppendLine(string.Format(periodBotReport,
            start.ToString(),
            end.ToString(),
            periodNewUsers,
            periodActiveUsers,
            periodUpdatesCount,
            periodExceptionsCount));

        return sb.ToString();
    }
}