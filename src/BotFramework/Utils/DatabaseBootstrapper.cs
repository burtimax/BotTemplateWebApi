using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using Microsoft.EntityFrameworkCore;

namespace BotFramework.Utils;
/// <summary>
/// Инициализатор данных в БД
/// </summary>
public class DatabaseBootstrapper
{
    /// <summary>
    /// Инициализация клэймов бота.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="claims"></param>
    public static async Task InitializeClaims(BotDbContext db, IEnumerable<ClaimValue> claims)
    {
        if (claims == null || claims.Any() == false) throw new ArgumentNullException(nameof(claims));
        
        foreach (ClaimValue claim in claims)
        {
            BotClaim? existedClaim = await db.Claims.SingleOrDefaultAsync(c => c.Name == claim.Name);

            if (existedClaim == null)
            {
                BotClaim newClaim = new()
                {
                    Name = claim.Name,
                    Description = claim.Description,
                    CreatedAt = DateTimeOffset.Now,
                };
                
                db.Claims.Add(newClaim);
            }
        }

        await db.SaveChangesAsync();
    }
}