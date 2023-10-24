﻿using System;
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
        string[] words = command.Split(' ', ',', '.')[1..];

        if (words == null || words.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }

        string userParam = words[0];
        BotUser? user = await _baseBotRepository.GetUserByIdentity(userParam);

        if (user == null)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userParam}].\n" + 
                                                        "Необходимо указать параметры команды.\n" +
                                                        "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }
        
        List<BotClaim> claimsToAdd = new List<BotClaim>();

        IEnumerable<string> claims = words[1 ..];

        if (claims == null || claims.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                              "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }
        
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

                claimsToAdd.Add(existed);
            }
        }

        await AddClaimsToUser(user.Id, claimsToAdd);
        
        IEnumerable<BotClaim>? userAllClaims = await _baseBotRepository.GetUserClaims(user.Id);
        await BotClient.SendTextMessageAsync(Chat.ChatId, ClaimsCommand.GenerateClaimsListString("Текущие разрешения пользователя", userAllClaims), ParseMode.Html);
    }

    /// <summary>
    /// Добавить разрешения пользователю.
    /// </summary>
    /// <param name="userId">ИД пользователя.</param>
    /// <param name="claims">Разрешения.</param>
    private async Task AddClaimsToUser(long userId, IEnumerable<BotClaim> claims)
    {
        foreach (var addClaim in claims)
        {
            await _baseBotRepository.AddClaimToUser(userId, addClaim.Name);
        }
    }
}