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
    private string botReport = "<b>Данные по боту</b>:\n" +
                               "<i>Кол-во польователей</i>: {0}\n" +
                               "<i>Кол-во запросов</i>: {1}\n" +
                               "<i>Кол-во ошибок</i>: {2}\n";

    /// <summary>
    /// 0 - Дата начала периода
    /// 1 - Дата окончания периода
    /// 2 - Кол-во новых пользователей
    /// 3 - Кол-во активных пользователей
    /// 4 - Кол-во запросов за период
    /// 5 - Кол-во ошибок за период
    /// </summary>
    private string periodBotReport = "<i>Начало</i> <b>{0}</b>\n" +
                                     "<i>Конец</i> <b>{1}</b>\n" +
                                     "<i>Новые пользователи</i>: {2}\n" +
                                     "<i>Активные пользователи</i>: {3}\n" +
                                     "<i>Кол-во запросов</i>: {4}\n" +
                                     "<i>Кол-во ошибок</i>: {5}\n";
    
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

        int hours = (end - start).Hours;

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