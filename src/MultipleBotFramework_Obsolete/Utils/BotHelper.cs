using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework_Obsolete.Db;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Repository;

namespace MultipleBotFramework_Obsolete.Utils;

public class BotHelper
{
    public delegate Task TaskAction<T>(T param);
    
    public static async Task ExecuteFor(BotDbContext db, long botId, IEnumerable<long> userTelegramIds, TaskAction<(BotUserEntity user, BotChatEntity chat)> action)
    {
        var users = await db.Users.Where(u => userTelegramIds.Contains(u.TelegramId))
            .AsNoTracking()
            .ToListAsync();
        
        await ExecuteFor(db, botId, users, action);
    }
    
    public static async Task ExecuteFor(BotDbContext db, long botId, long userTelegramId, TaskAction<(BotUserEntity user, BotChatEntity chat)> action)
    {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.TelegramId == userTelegramId);
        await ExecuteFor(db, botId, new []{ user }, action);
    }

    public static async Task ExecuteFor(BotDbContext db, long botId, string claimName, TaskAction<(BotUserEntity user, BotChatEntity chat)> action)
    {
        IBaseBotRepository repository = new BaseBotRepository(db);
        var users = (await repository.GetUsersByClaim(botId, claimName));
        await ExecuteFor(db, botId, users, action);
    }
    
    public static async Task ExecuteFor(BotDbContext db, long botId, IEnumerable<BotUserEntity> users, TaskAction<(BotUserEntity user, BotChatEntity chat)> action)
    {
        if (users == null) throw new ArgumentNullException(nameof(users));
        
        var usersIds = users.Select(u => u.Id);
        var chats = await db.Chats.Where(c => c.BotId == botId && usersIds.Contains(c.BotUserId)).ToListAsync();

        if (users.Count() != chats.Count) throw new Exception();

        foreach (var user in users)
        {
            var chat = chats.First(c => c.BotUserId == user.Id);
            await action.Invoke((user, chat));
        }
    }
    
    public static async Task ExecuteForAllUsers(BotDbContext db, long botId, TaskAction<(BotUserEntity user, BotChatEntity chat)> action)
    {
        int usersCount = await db.Users.CountAsync();
        int offset = 0, limit = 100;

        while (offset < usersCount)
        {
            IEnumerable<BotUserEntity> users = await db.Users
                .OrderBy(u => u.Id)
                .Skip(offset)
                .Take(limit)
                .AsNoTracking()
                .ToListAsync();

            await ExecuteFor(db, botId, users, action);

            offset += limit;
        }
    }
    
}