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
/// Команда для отображения всех возможных разрешений бота.
/// Доступна только суперадмину.
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
        string[] claimIds = command.Split(' ', ',', '.')[1..];

        if (claimIds == null || claimIds.Any() == false)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, "Необходимо указать параметры команды.\n" +
                                                        "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }

        string userParam = claimIds[0];
        BotUser? user = await GetUserByIdentity(userParam);

        if (user == null)
        {
            await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найден пользователь [{userParam}].\n" + 
                                                        "Необходимо указать параметры команды.\n" +
                                                        "Например [/set {@user} {число|строка} {число|строка} ...]");
            return;
        }

        try
        {
            foreach (var claimId in claimIds)
            {
                if (long.TryParse(claimId, out var numberId))
                {
                    BotClaim? existed = await _baseBotRepository.GetClaimById(numberId);
                    existed ??= await _baseBotRepository.GetClaimByName(claimId);

                    if (existed == null)
                    {
                        throw new NotFoundBotClaim(claimId);
                    }

                    await _baseBotRepository.AddClaimToUser(user.Id, existed.Name);
                }
            }
        }
        catch (Exception e) when(e is NotFoundBotClaim)
        {
            foreach (string claimId in claimIds)
            {
                if (long.TryParse(claimId, out var numberId))
                {
                    BotClaim? existed = await _baseBotRepository.GetClaimById(numberId);
                    existed ??= await _baseBotRepository.GetClaimByName(claimId);

                    if (existed == null)
                    {
                        await BotClient.SendTextMessageAsync(Chat.ChatId, $"Не найдено разрешение [{claimId}]");
                        break;
                    }

                    await _baseBotRepository.RemoveClaimFromUser(user.Id, existed.Name);
                }
            }

            return;
        }

        IEnumerable<BotClaim>? userAllClaims = await _baseBotRepository.GetUserClaims(user.Id);
        await BotClient.SendTextMessageAsync(Chat.ChatId, ClaimsCommand.GenerateClaimsListString("Текущие разрешения пользователя", userAllClaims), ParseMode.Html);
    }

    /// <summary>
    /// Получить пользователя по идентификатору.
    /// </summary>
    /// <param name="userIdentity">Строка [@user] или число [Id].</param>
    /// <returns></returns>
    private async Task<BotUser?> GetUserByIdentity(string userIdentity)
    {
        if (long.TryParse(userIdentity, out var numberId))
        {
            return await _baseBotRepository.GetUser(numberId);
        }

        return await _baseBotRepository.GetUser(userIdentity);
    }
}