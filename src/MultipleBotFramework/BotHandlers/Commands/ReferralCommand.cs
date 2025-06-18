using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Db.ReferralDb.Entity;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Services.Referral;
using MultipleBotFramework.Utils;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.BotHandlers.Commands;

/// <summary>
/// Команда уведомления для всех пользователей бота.
/// </summary>
[BotHandler(command:Name,version:1.0f)]
public class ReferralCommand : BaseBotHandler
{
    public const string Name = "/referral";

    private readonly IReferralService _referralService;
    
    public ReferralCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _referralService = serviceProvider.GetService<IReferralService>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        if (_referralService is null)
        {
            await Answer("Реферальная система отключена");
            return;
        }

        ReferralParticipant participant = await _referralService.GetParticipantWithCampaigns(BotId, User.TelegramId);
        
        var defaultCampaign = participant.Campaigns.FirstOrDefault(c => c.Name == ReferralCampaign.DefaultName);

        if (defaultCampaign is null)
        {
            await Answer("Referral campaign not found");
            throw new Exception("Referral campaign not found");
        }
        
        BotEntity? bot = BotDbContext.Bots.FirstOrDefault(b => b.Id == BotId);

        if (bot == null)
        {
            throw new Exception($"Bot not found [{BotId}]");
        }

        string botLink = BotConstants.TelegramLinkFormat.F(bot.Username.Trim('@', ' '));
        string referralLink = botLink + $"?start={defaultCampaign.Code}";
        await Answer(MsgReferralCommand.F(referralLink, defaultCampaign.ReferralCount));
    }
}