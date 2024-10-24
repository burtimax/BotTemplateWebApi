using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MultipleBotFramework_Obsolete.Attributes;
using MultipleBotFramework_Obsolete.Base;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Exceptions;
using MultipleBotFramework_Obsolete.Options;
using MultipleBotFramework_Obsolete.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework_Obsolete.BotHandlers.Commands;

/// <summary>
/// Команда для добавления разрешений пользователю бота.
/// /set @user {claim_name|claim_id} {claim_name|claim_id} ...
/// </summary>
[BotCommand(Name, version: 1.0f, RequiredUserClaims = new []{BotConstants.BaseBotClaims.BotUserClaimCreate})]
public class SetClaimsCommand: BaseBotCommand
{
    internal const string Name = "/set";

    private readonly BotConfiguration _botConfiguration;
    private readonly IBaseBotRepository _baseBotRepository;
    
    public SetClaimsCommand(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _botConfiguration = serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _baseBotRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
    }

    public override async Task HandleBotRequest(Update update)
    {
        string command = update.Message.Text?.Trim(' ', '.');
        string[] words = command.Split(' ', ',')[1..];

        if (words == null || words.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }

        string userIdentity = words[0];
        BotUserEntity? user = await _baseBotRepository.GetUserByIdentity(BotId, userIdentity);

        if (user == null)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userIdentity}].\n" + 
                                                        "Необходимо указать параметры команды.\n" +
                                                        "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }
        
        List<BotClaimEntity> claimsToAdd = new List<BotClaimEntity>();

        IEnumerable<string> claims = words[1 ..];

        if (claims == null || claims.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                              "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }
        
        foreach (var claimId in claims)
        {
            BotClaimEntity? existed;
            if (long.TryParse(claimId, out var numberId))
            {
                existed = await _baseBotRepository.GetClaimById(numberId);
            }
            else
            {
                existed = await _baseBotRepository.GetClaimByName(claimId);
            }
            
            if (existed == null)
            {
                await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найдено разрешение [{claimId}]");
                throw new NotFoundBotClaim(claimId);
            }

            // Нельзя добавлять супер разрешение другим. Оно только у админа.
            if(existed.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty) continue;
            
            claimsToAdd.Add(existed);
        }

        await AddClaimsToUser(user.Id, claimsToAdd);
        
        IEnumerable<BotClaimEntity>? userAllClaims = await _baseBotRepository.GetUserClaims(BotId, user.Id);
        await BotClient.SendTextMessageAsync(Chat.ChatId, ClaimsCommand.GenerateClaimsListString(userAllClaims, $"Текущие разрешения пользователя {userIdentity}"), parseMode:ParseMode.Html);
    }

    /// <summary>
    /// Добавить разрешения пользователю.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="claims">Разрешения.</param>
    private async Task AddClaimsToUser(long userId, IEnumerable<BotClaimEntity> claims)
    {
        foreach (var addClaim in claims)
        {
            await _baseBotRepository.AddClaimToUser(BotId, userId, addClaim.Name);
        }
    }
}