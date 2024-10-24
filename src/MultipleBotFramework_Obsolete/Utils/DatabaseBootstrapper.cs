using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework_Obsolete.Db;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Dto;

namespace MultipleBotFramework_Obsolete.Utils;
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
            BotClaimEntity? existedClaim = await db.Claims.SingleOrDefaultAsync(c => c.Name == claim.Name);

            if (existedClaim == null)
            {
                BotClaimEntity newClaimEntity = new()
                {
                    Name = claim.Name,
                    Description = claim.Description,
                    CreatedAt = DateTimeOffset.Now,
                };
                
                db.Claims.Add(newClaimEntity);
            }
        }

        await db.SaveChangesAsync();
    }
}