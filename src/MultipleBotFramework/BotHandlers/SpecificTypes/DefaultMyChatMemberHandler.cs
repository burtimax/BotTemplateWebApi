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
            await UpdateUserStatus(data.NewChatMember.Status);
        }

        // Если пользователь заблокировал бота.
        if (data.NewChatMember is ChatMemberBanned)
        {
            // ToDo можно сюда сделать вызов события какого-нибудь.
        }
    }

    private async Task UpdateUserStatus(string status)
    {
        if (User is null) return;
        User.Status = status;
        BotDbContext.Users.Update(User);
        await BotDbContext.SaveChangesAsync();
    }
}