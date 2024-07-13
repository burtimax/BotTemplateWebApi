using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Repository;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Utils;

public class BotHelper
{
    public delegate Task TaskAction<T>(T param);
    
    public static async Task ExecuteFor(BotDbContext db, IEnumerable<long> userTelegramIds, TaskAction<(BotUser user, BotChat chat)> action)
    {
        var users = await db.Users.Where(u => userTelegramIds.Contains(u.TelegramId))
            .AsNoTracking()
            .ToListAsync();
        
        await ExecuteFor(db, users, action);
    }
    
    public static async Task ExecuteFor(BotDbContext db, long userTelegramId, TaskAction<(BotUser user, BotChat chat)> action)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.TelegramId == userTelegramId);
        await ExecuteFor(db, new []{ user }, action);
    }

    public static async Task ExecuteFor(BotDbContext db, string claimName, TaskAction<(BotUser user, BotChat chat)> action)
    {
        IBaseBotRepository repository = new BaseBotRepository(db);
        var users = (await repository.GetUsersByClaim(claimName));
        await ExecuteFor(db, users, action);
    }
    
    public static async Task ExecuteFor(BotDbContext db, IEnumerable<BotUser> users, TaskAction<(BotUser user, BotChat chat)> action)
    {
        if (users == null) throw new ArgumentNullException(nameof(users));
        
        var usersIds = users.Select(u => u.Id);
        var chats = await db.Chats.Where(c => usersIds.Contains(c.BotUserId)).ToListAsync();

        if (users.Count() != chats.Count) throw new Exception();

        foreach (var user in users)
        {
            var chat = chats.First(c => c.BotUserId == user.Id);
            await action.Invoke((user, chat));
        }
    }
    
    public static async Task ExecuteForAllUsers(BotDbContext db, TaskAction<(BotUser user, BotChat chat)> action)
    {
        int usersCount = await db.Users.CountAsync();
        int offset = 0, limit = 100;

        while (offset < usersCount)
        {
            IEnumerable<BotUser> users = await db.Users
                .OrderBy(u => u.Id)
                .Skip(offset)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();

            await ExecuteFor(db, users, action);

            offset += limit;
        }
    }
    
}