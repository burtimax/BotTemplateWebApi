using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Interfaces;
using BotFramework.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;

namespace BotFramework.Implementation
{
    public class BaseBotRepository : IBaseBotRepository
    {
        private readonly BotDbContext _db;
        
        public BaseBotRepository(BotDbContext db)
        {
            _db = db;
        }
        
        public Task<bool> IsUserInBot(long userId)
        {
            return _db.Users.AnyAsync(u => u.TelegramId == userId);
        }

        public Task<bool> IsChatInBot(ChatId chatId)
        {
            if (chatId.Username != null)
            {
                return _db.Chats.AnyAsync(c => c.TelegramUsername == chatId.Username);
            }

            if (chatId.Identifier != null)
            {
                return _db.Chats.AnyAsync(c => c.TelegramId == chatId.Identifier);
            }

            return Task.FromResult(false);
        }

        public async Task AddUserToBot(User user, string role = null)
        {
            await _db.Users.AddAsync(user.ToBotUserEntity(role));
        }

        public async Task AddChatToBot(Chat chat, BotUser chatOwner)
        {
            await _db.Chats.AddAsync(chat.ToBotChatEntity(chatOwner.Id));
        }

        public Task SaveMessage(InboxMessage inboxMessage)
        {
            throw new System.NotImplementedException();
        }

        public Task<BotUser> GetUser(long userId)
        {
            return _db.Users.SingleAsync(u => u.TelegramId == userId);
        }

        public Task<BotChat> GetChat(ChatId chatId)
        {
            if (chatId.Username != null)
            {
                return _db.Chats.SingleAsync(c => c.TelegramUsername == chatId.Username);
            }
            else
            {
                return _db.Chats.SingleAsync(c => c.TelegramId == chatId.Identifier);
            }
        }

        public Task SetChatData(BotChat chat)
        {
            throw new System.NotImplementedException();
        }
    }
}