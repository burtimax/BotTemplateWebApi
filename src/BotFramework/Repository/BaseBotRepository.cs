using System.Threading.Tasks;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Dto;
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

        public Task<BotUser> GetUser(long userId)
        {
            return _db.Users.SingleAsync(u => u.TelegramId == userId);
        }

        public async Task<BotUser> UpsertUser(User user)
        {
            var existedUser = await GetUser(user.Id);

            if (existedUser == null)
            {
                BotUser newUser = user.ToBotUserEntity();
                await _db.Users.AddAsync(newUser);
                return newUser;
            }

            existedUser.TelegramFirstname = user.FirstName;
            existedUser.TelegramLastname = user.LastName;
            existedUser.TelegramUsername = user.Username;

            await _db.SaveChangesAsync();

            return existedUser;
        }

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

        public async Task<BotChat> AddChat(Chat chat, BotUser chatOwner)
        {
            BotChat newChat = chat.ToBotChatEntity(chatOwner.Id);
            newChat.States.CurrentState = BotConstants.StartState;
            _db.Chats.Add(newChat);
            await _db.SaveChangesAsync();
            return newChat;
        }

        public async Task<BotMessage> AddMessage(SaveMessageDto messageDto)
        {
            BotMessage newMessage = new BotMessage()
            {
                TelegramMessageId = messageDto.TelegramId,
                BotChatId = messageDto.BotChatId,
                Type = messageDto.MessageType,
                Content = messageDto.MessageContent
            };

            _db.Messages.Add(newMessage);
            await _db.SaveChangesAsync();
            return newMessage;
        }
    }
}