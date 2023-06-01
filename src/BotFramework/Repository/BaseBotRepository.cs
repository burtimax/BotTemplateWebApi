using System.Collections.Generic;
using System.Linq;
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


        /// <inheritdoc />
        public Task<BotUser> GetUser(long userId)
        {
            return _db.Users.SingleAsync(u => u.TelegramId == userId);
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
        public async Task<BotChat> AddChat(Chat chat, BotUser chatOwner)
        {
            BotChat newChat = chat.ToBotChatEntity(chatOwner.Id);
            newChat.States.CurrentState = BotConstants.StartState;
            _db.Chats.Add(newChat);
            await _db.SaveChangesAsync();
            return newChat;
        }

        /// <inheritdoc />
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