using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotFramework.Repository
{
    public class BaseBotRepository : IBaseBotRepository
    {
        private readonly BotDbContext _db;

        public BaseBotRepository(BotDbContext db)
        {
            _db = db;
        }
        
        /// <inheritdoc />
        public Task<BotUser?> GetUser(long userId)
        {
            return _db.Users
                .SingleOrDefaultAsync(u => u.TelegramId == userId);
        }

        /// <inheritdoc />
        public async Task<BotUser?> GetUser(string username)
        {
            username = username.Trim('@');
            return await _db.Users.SingleOrDefaultAsync(u => u.TelegramUsername == username);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BotUser>?> SearchUsers(string searchStr, int skip, int limit)
        {
            if (searchStr == null) return null;

            string[] words = searchStr
                .ToLower()
                .Trim(' ', '.')
                .Split(' ', ',', ' ');

            IQueryable<BotUser> query = _db.Users.AsQueryable();
            foreach (var word in words)
            {
                query = query.Where(u => EF.Functions.Like(
                    u.TelegramId.ToString() + " " + u.TelegramFirstname.ToLower() + " " + u.TelegramLastname.ToLower() + " " + u.TelegramUsername.ToLower(), 
                    $"%{word.Trim(' ', ',', '@')}%"))
                    .AsQueryable();
            }

            return await query.Skip(skip).Take(limit).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<BotUser?> GetUserByIdentity(string userIdentity)
        {
            if (long.TryParse(userIdentity, out var numberId))
            {
                return await GetUser(numberId);
            }

            return await GetUser(userIdentity);
        }

        /// <inheritdoc />
        public async Task BlockUsers(params long[] userIds)
        {
            var users = await _db.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.IsBlocked = true;
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UnblockUsers(params long[] userIds)
        {
            var users = await _db.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.IsBlocked = false;
            }

            await _db.SaveChangesAsync();
        }

        private async Task<bool> IsUserExists(long userId)
        {
            BotUser? u = await _db.Users.FirstOrDefaultAsync(u => u.TelegramId == userId);
            return u != null;
        }

        /// <inheritdoc />
        public async Task<BotUser> UpsertUser(User user)
        {
            bool isUserExisted = await IsUserExists(user.Id);

            if (isUserExisted == false)
            {
                BotUser newUser = user.ToBotUserEntity();
                newUser.CreatedAt = DateTimeOffset.Now;
                _db.Users.Add(newUser);
                await _db.SaveChangesAsync();
                return newUser;
            }

            var existedUser = await GetUser(user.Id);
            existedUser.TelegramFirstname = user.FirstName;
            existedUser.TelegramLastname = user.LastName;
            existedUser.TelegramUsername = user.Username;

            await _db.SaveChangesAsync();

            return existedUser;
        }

        /// <inheritdoc />
        public Task<BotChat?> GetChat(ChatId chatId)
        {
            if (chatId.Username != null)
            {
                return _db.Chats.SingleOrDefaultAsync(c => c.TelegramUsername == chatId.Username);
            }
            else
            {
                return _db.Chats.SingleOrDefaultAsync(c => c.TelegramId == chatId.Identifier);
            }
        }

        /// <inheritdoc />
        public Task<BotChat?> GetChat(long botUserId)
        {
            return _db.Chats.SingleOrDefaultAsync(c => c.BotUserId == botUserId);
        }

        /// <inheritdoc />
        public async Task<BotChat> AddChat(Chat chat, BotUser chatOwner)
        {
            BotChat newChat = chat.ToBotChatEntity(chatOwner.Id);
            newChat.States.CurrentState = BotConstants.StartState;
            newChat.CreatedAt = DateTimeOffset.Now;
            _db.Chats.Add(newChat);
            await _db.SaveChangesAsync();
            return newChat;
        }

        /// <inheritdoc />
        public async Task<BotUpdate> AddUpdate(SaveUpdateDto updateDto)
        {
            BotUpdate newUpdate = new BotUpdate()
            {
                TelegramMessageId = updateDto.TelegramId,
                BotChatId = updateDto.BotChatId,
                Type = updateDto.Type,
                Content = updateDto.Content
            };

            newUpdate.CreatedAt = DateTimeOffset.Now;
            _db.Updates.Add(newUpdate);
            await _db.SaveChangesAsync();
            return newUpdate;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BotClaim>?> GetUserClaims(long userId)
        {
            List<BotClaim> claims = await _db.UserClaims
                .Include(uc => uc.Claim)
                .Where(u => u.UserId == userId)
                .Select(uc => uc.Claim)
                .ToListAsync();

            // У Брюса Всемогущего есть все разрешения.
            if (HasBruceAlmightyClaim(claims))
            {
                return await GetAllClaims();
            }
            
            if (claims.Any() == false) return null;

            return claims;
        }

        /// <inheritdoc />
        public async Task AddClaimToUser(long userId, string claim)
        {
            BotClaim? existed = await GetClaimByName(claim);

            if (existed == null) throw new NotFoundBotClaim(claim);

            BotUserClaim botUserClaim = new()
            {
                UserId = userId,
                ClaimId = existed.Id,
            };

            await _db.UserClaims.AddAsync(botUserClaim);
        }

        /// <inheritdoc />
        public async Task RemoveClaimFromUser(long userId, string claim)
        {
            BotClaim? existed = await GetClaimByName(claim);

            if (existed == null) throw new NotFoundBotClaim(claim);

            BotUserClaim? botUserClaim =
                await _db.UserClaims.SingleOrDefaultAsync(uc => uc.UserId == userId && uc.ClaimId == existed.Id);

            if (botUserClaim == null) return;

            _db.UserClaims.Remove(botUserClaim);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasUserClaims(long userId, params string[] claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));
            
            List<BotUserClaim> userClaims = await _db.UserClaims
                .Include(uc => uc.Claim)
                .Where(uc => claims.Contains(uc.Claim.Name))
                .ToListAsync();

            if (userClaims == null) return false;
            if (userClaims.Count > claims.Length) throw new Exception();

            // У брюса всемогущего есть все разрешения.
            if (HasBruceAlmightyClaim(userClaims.Select(uc => uc.Claim)))
            {
                return true;
            }
            
            foreach (string claim in claims)
            {
                bool found = userClaims.Any(uc => uc.Claim.Name == claim);

                if (found == false) return false;
            }

            return true;
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<BotClaim>> GetAllClaims(bool hideBruceClaim = false)
        {
            var claims = _db.Claims.Where(c => true);

            if (hideBruceClaim)
            {
                claims = claims.Where(c => c.Name != BotConstants.BaseBotClaims.IAmBruceAlmighty);
            }

            return await claims.ToListAsync();
        }

        /// <inheritdoc />
        public Task<BotClaim?> GetClaimByName(string name)
        {
            return _db.Claims.SingleOrDefaultAsync(c => c.Name == name);
        }
        
        /// <inheritdoc />
        public Task<BotClaim?> GetClaimById(long id)
        {
            return _db.Claims.SingleOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Если это Брюс Всемогущий????
        /// </summary>
        /// <param name="claims"></param>
        /// <returns>TRUE - если пользователь - Брюс Всемогущий!!!!</returns>
        private bool HasBruceAlmightyClaim(IEnumerable<BotClaim> claims) =>
            claims.Any(c => c.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty);
    }
}