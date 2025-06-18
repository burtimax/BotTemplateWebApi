using System;
using System.Threading.Tasks;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Enums;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.SpecificTypes;

[BotHandler(updateTypes:new [] { UpdateType.MyChatMember }, version:0)]
public class DefaultMyChatMemberHandler : BaseBotHandler
{
    public DefaultMyChatMemberHandler(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task HandleBotRequest(Update update)
    {
        ChatMemberUpdated data = update.MyChatMember!;

        if (data.NewChatMember is not null)
        {
            await UpdateChatStatus(data.NewChatMember.Status);
        }

        // Если пользователь заблокировал бота.
        if (data.NewChatMember is ChatMemberBanned)
        {
            // ToDo можно сюда сделать вызов события какого-нибудь.
        }
    }

    private async Task UpdateChatStatus(string status)
    {
        if (Chat is null) return;
        Chat.Status = status;
        BotDbContext.Chats.Update(Chat);
        await BotDbContext.SaveChangesAsync();
    }
}