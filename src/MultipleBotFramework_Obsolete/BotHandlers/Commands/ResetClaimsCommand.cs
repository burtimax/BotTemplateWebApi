﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFramework_Obsolete.Attributes;
using MultipleBotFramework_Obsolete.Base;
using MultipleBotFramework_Obsolete.Db.Entity;
using MultipleBotFramework_Obsolete.Exceptions;
using MultipleBotFramework_Obsolete.Repository;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework_Obsolete.BotHandlers.Commands;

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
        string[] words = command.Split(' ', ',')[1..];

        if (words == null || words.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/reset {@user} {число|строка} {число|строка} ...]");
            return;
        }

        string userParam = words[0];
        BotUserEntity? user = await _baseBotRepository.GetUserByIdentity(BotId, userParam);

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
        
        List<BotClaimEntity> claimsToDelete = new List<BotClaimEntity>();

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

            // Нельзя удалять супер ращрешение у других. Оно только у админа.
            if(existed.Name == BotConstants.BaseBotClaims.IAmBruceAlmighty) continue;
            
            claimsToDelete.Add(existed);
        }

        await RemoveClaimsFromUser(user.Id, claimsToDelete);
        
        IEnumerable<BotClaimEntity>? userAllClaims = await _baseBotRepository.GetUserClaims(BotId, user.Id);
        await BotClient.SendTextMessageAsync(Chat.ChatId, ClaimsCommand.GenerateClaimsListString(userAllClaims, "Текущие разрешения пользователя"), parseMode:ParseMode.Html);
    }

    /// <summary>
    /// Отнять разрешения у пользователя.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="claims">Разрешения, которые нужно отнять.</param>
    private async Task RemoveClaimsFromUser(long userId, IEnumerable<BotClaimEntity> claims)
    {
        foreach (var removeClaim in claims)
        {
            if (await _baseBotRepository.HasUserClaims(BotId, userId, removeClaim.Name))
            {
                await _baseBotRepository.RemoveClaimFromUser(BotId, userId, removeClaim.Name);
            }
        }
    }
    
}