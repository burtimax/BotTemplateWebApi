using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db.Entity;
using BotFramework.Exceptions;
using BotFramework.Options;
using BotFramework.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.BotCommands;

/// <summary>
/// Команда для удаления разрешений пользователя бота.
/// /reset @user {claim_name|claim_id} {claim_name|claim_id} ...
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserClaimDelete})]
public class ResetClaimsCommand: BaseBotCommand
{
    internal const string Name = "/reset";

    private readonly IBaseBotRepository _baseBotRepository;
    
    public ResetClaimsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string command = update.Message.Text?.Trim(' ', '.');
        string[] words = command.Split(' ', ',', '.')[1..];

        if (words == null || words.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/reset {@user} {число|строка} {число|строка} ...]");
            return;
        }

        string userParam = words[0];
        BotUser? user = await _baseBotRepository.GetUserByIdentity(userParam);

        if (user == null)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userParam}].\n" + 
                                                        "Необходимо указать параметры команды.\n" +
                                                        "Например [/reset {@user} {число|строка} {число|строка} ...]");
            return;
        }
        
        IEnumerable<string> claims = words[1 ..];

        if (claims == null || claims.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                              "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }
        
        List<BotClaim> claimsToDelete = new List<BotClaim>();

        foreach (var claimId in claims)
        {
            if (long.TryParse(claimId, out var numberId))
            {
                BotClaim? existed = await _baseBotRepository.GetClaimById(numberId);
                existed ??= await _baseBotRepository.GetClaimByName(claimId);

                if (existed == null)
                {
                    await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найдено разрешение [{claimId}]");
                    throw new NotFoundBotClaim(claimId);
                }

                claimsToDelete.Add(existed);
            }
        }

        await RemoveClaimsFromUser(user.Id, claimsToDelete);
        
        IEnumerable<BotClaim>? userAllClaims = await _baseBotRepository.GetUserClaims(user.Id);
        await BotClient.SendTextMessageAsync(Chat.ChatId, ClaimsCommand.GenerateClaimsListString("Текущие разрешения пользователя", userAllClaims), ParseMode.Html);
    }

    /// <summary>
    /// Отнять разрешения у пользователя.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="claims">Разрешения, которые нужно отнять.</param>
    private async Task RemoveClaimsFromUser(long userId, IEnumerable<BotClaim> claims)
    {
        foreach (var removeClaim in claims)
        {
            if (await _baseBotRepository.HasUserClaims(userId, removeClaim.Name))
            {
                await _baseBotRepository.RemoveClaimFromUser(userId, removeClaim.Name);
            }
        }
    }
    
}