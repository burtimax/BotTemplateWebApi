﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Exceptions;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Repository
{
    public class BaseBotRepository : IBaseBotRepository
    {
        private readonly BotDbContext _db;

        public BaseBotRepository(BotDbContext db)
        {
            _db = db;
        }
        
        /// <inheritdoc />
        public Task<BotUserEntity?> GetUser(long botId, long userTelegramId)
        {
            return _db.Users
                .SingleOrDefaultAsync(u => u.BotId == botId 
                                           && u.TelegramId == userTelegramId);
        }

        /// <inheritdoc />
        public async Task<BotUserEntity?> GetUser(long botId, string username)
        {
            username = username.Trim('@');
            return await _db.Users.SingleOrDefaultAsync(u => u.BotId == botId 
                                                             && u.TelegramUsername == username);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BotUserEntity>?> SearchUsers(long botId, string searchStr, int skip, int limit)
        {
            if (searchStr == null) return null;

            string[] words = searchStr
                .ToLower()
                .Trim(' ', '.')
                .Split(' ', ',', ' ');

            IQueryable<BotUserEntity> query = _db.Users.AsQueryable();
            foreach (var word in words)
            {
                query = query.Where(u => u.BotId == botId
                && EF.Functions.Like(
                    u.TelegramId.ToString() + " " + u.TelegramFirstname.ToLower() + " " + u.TelegramLastname.ToLower() + " " + u.TelegramUsername.ToLower(), 
                    $"%{word.Trim(' ', ',', '@')}%"))
                    .AsQueryable();
            }

            return await query.Skip(skip).Take(limit).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<BotUserEntity?> GetUserByIdentity(long botId, string userIdentity)
        {
            if (long.TryParse(userIdentity, out var numberId))
            {
                return await GetUser(botId, numberId);
            }

            return await GetUser(botId, userIdentity);
        }

        /// <inheritdoc />
        public async Task BlockUsers(long botId, params long[] userIds)
        {
            var users = await _db.Users.Where(u => u.BotId == botId 
            && userIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.IsBlocked = true;
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task UnblockUsers(long botId, params long[] userIds)
        {
            var users = await _db.Users.Where(u => u.BotId == botId 
            && userIds.Contains(u.Id)).ToListAsync();
            foreach (var user in users)
            {
                user.IsBlocked = false;
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BotUserEntity>> GetUsersByClaim(long botId, string claimName)
        {
            BotClaimEntity claimEntity = await GetClaimByName(claimName) ?? throw new NotFoundBotClaim(claimName);
            BotClaimEntity superClaimEntity = await GetClaimByName(BotConstants.BaseBotClaims.IAmBruceAlmighty) ?? throw new NotFoundBotClaim(claimName);

            return (await _db.UserClaims
                .Include(uc => uc.User)
                .Where(uc => uc.BotId == botId && (uc.ClaimId == claimEntity.Id || uc.ClaimId == superClaimEntity.Id))
                .Select(uc => uc.User)
                .ToListAsync());
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BotUserEntity>> GetUsersByRole(string role)
        {
            return await _db.Users.Where(u => u.Role == role).ToListAsync();
        }

        private async Task<bool> IsUserExists(long botId, long userId)
        {
            BotUserEntity? u = await _db.Users.FirstOrDefaultAsync(u => u.BotId == botId && u.TelegramId == userId);
            return u != null;
        }

        /// <inheritdoc />
        public async Task<BotUserEntity?> UpsertUser(long botId, User user, ITelegramBotClient botClient)
        {
            if (user == null) return null;
            
            bool isUserExisted = await IsUserExists(botId, user.Id);

            if (isUserExisted == false)
            {
                BotUserEntity newUserEntity = user.ToBotUserEntity(botId);
                newUserEntity.CreatedAt = DateTimeOffset.Now;
                _db.Users.Add(newUserEntity);
                await _db.SaveChangesAsync();
                return newUserEntity;
            }

            var existedUser = (await GetUser(botId, user.Id))!;
            existedUser.TelegramFirstname = user.FirstName;
            existedUser.TelegramLastname = user.LastName;
            existedUser.TelegramUsername = user.Username;

            await _db.SaveChangesAsync();

            return existedUser;
        }

        // public async Task<BotUserEntity?> UpdatePhotos(long botId, long telegramUserId, UserProfilePhotos photos)
        // {
        //     
        // }

        public async Task<BotChatEntity?> UpsertChat(long botId, Chat chat, User? user)
        {
            if (chat == null) return null;
            BotUserEntity? botUser = null;
            if (user is not null)
            {
                botUser = await GetUser(botId, user!.Id);
            }
            
            BotChatEntity? existed = await GetChatById(botId, chat.Id);
            
            if (existed is null)
            {
                existed = await AddChat(botId, chat, botUser);
            }

            existed.TelegramUsername = chat.Username ?? null;
            existed.Title = chat.Title ?? user?.FirstName + user?.LastName ?? null;
            if (string.IsNullOrEmpty(chat.Type) == false) existed.Type = chat.Type;

            _db.Chats.Update(existed);
            await _db.SaveChangesAsync();

            return existed;
        }
      
        public Task<BotChatEntity?> GetChatById(long botId, long chatId)
        {
            return _db.Chats.FirstOrDefaultAsync(c => c.BotId == botId
                                                       && c.TelegramId == chatId);
        }

        /// <inheritdoc />
        public async Task<BotChatEntity?> AddChat(long botId, Chat? chat, BotUserEntity? chatOwner)
        {
            if (chat == null) return null;
            
            BotChatEntity newChatEntity = chat.ToBotChatEntity(botId, chatOwner?.Id);
            newChatEntity.States.CurrentState = BotConstants.StartState;
            newChatEntity.CreatedAt = DateTimeOffset.Now;
            _db.Chats.Add(newChatEntity);
            await _db.SaveChangesAsync();
            return newChatEntity;
        }

        /// <inheritdoc />
        public async Task<BotUserEntity?> GetUserById(long botId, long userId)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.BotId == botId && u.Id == userId);
        }

        /// <inheritdoc />
        public async Task<bool> IsUserOwner(long botId, long userTelegramId)
        {
            return (await _db.BotOwners.FirstOrDefaultAsync(bo => bo.BotId == botId && bo.UserTelegramId == userTelegramId)) is not null;
        }
        
        /// <inheritdoc />
        public async Task<IEnumerable<BotClaimEntity>?> GetUserClaims(long botId, long userId)
        {
            List<BotClaimEntity> claims = await _db.UserClaims
                .Include(uc => uc.Claim)
                .Where(u => u.BotId == botId 
                                            && u.UserId == userId)
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
        public async Task AddClaimToUser(long botId, long userId, string claim)
        {
            BotClaimEntity? existed = await GetClaimByName(claim);

            if (existed == null) throw new NotFoundBotClaim(claim);

            bool hasUserClaim = await HasUserClaims(botId, userId, claim);
            if(hasUserClaim) return;
            
            BotUserClaimEntity botUserClaimEntity = new()
            {
                BotId = botId,
                UserId = userId,
                ClaimId = existed.Id,
            };

            await _db.UserClaims.AddAsync(botUserClaimEntity);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task RemoveClaimFromUser(long botId, long userId, string claim)
        {
            BotClaimEntity? existed = await GetClaimByName(claim);

            if (existed == null) throw new NotFoundBotClaim(claim);

            BotUserClaimEntity? botUserClaim =
                await _db.UserClaims.SingleOrDefaultAsync(uc => uc.BotId == botId && uc.UserId == userId && uc.ClaimId == existed.Id);

            if (botUserClaim == null) return;

            _db.UserClaims.Remove(botUserClaim);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> HasUserClaims(long botId, long userId, params string[] claims)
        {
            if (claims == null) throw new ArgumentNullException(nameof(claims));
            
            List<BotUserClaimEntity> userClaims = await _db.UserClaims
                .Include(uc => uc.Claim)
                .Where(uc => uc.BotId == botId && claims.Contains(uc.Claim.Name))
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
        public async Task<IEnumerable<BotClaimEntity>> GetAllClaims(bool hideBruceClaim = false)
        {
            var claims = _db.Claims.Where(c => true);

            return await claims.ToListAsync();
        }

        /// <inheritdoc />
        public Task<BotClaimEntity?> GetClaimByName(string name)
        {
            return _db.Claims.SingleOrDefaultAsync(c => c.Name == name);
        }
        
        /// <inheritdoc />
        public Task<BotClaimEntity?> GetClaimById(long id)
        {
            return _db.Claims.SingleOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Если это Брюс Всемогущий????
        /// </summary>
        /// <param name="claims"></param>
        /// <returns>TRUE - если пользователь - Брюс Всемогущий!!!!</returns>
        private bool HasBruceAlmightyClaim(IEnumerable<BotClaimEntity> claims) =>
            claims.Any(c => c.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty);
    }
}