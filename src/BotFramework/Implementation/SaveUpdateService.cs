using System.Text.Json;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using BotFramework.Extensions;
using BotFramework.Interfaces;
using BotFramework.Repository;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Implementation;

/// <inheritdoc />
public class SaveUpdateService : ISaveUpdateService
{
    private readonly IBaseBotRepository _botRepository;
    
    public SaveUpdateService(IBaseBotRepository botRepository)
    {
        _botRepository = botRepository;
    }
    
    /// <inheritdoc />
    public async Task SaveUpdateInBotHistory(BotUser user, BotChat chat, Update update)
    {
        SaveUpdateDto saveUpdateDto = new()
        {
            BotChatId = chat.Id,
            Type = update.Type.ToString(),
            TelegramId = update.Id,
            Content = "NULL"
        };

        saveUpdateDto.Content = update.Type switch
        {
            UpdateType.Message => update.Message?.Text,
            UpdateType.CallbackQuery => update.CallbackQuery?.Data,
            UpdateType.EditedMessage => update.EditedMessage?.Text,
            UpdateType.Poll => update.Poll?.Question,
            UpdateType.ChannelPost => $"{update.ChannelPost?.Caption} \n {update.ChannelPost?.Text}",
            UpdateType.ChatMember => $"{update.ChatMember?.From?.Id} : {update.ChatMember?.From?.Username}",
            UpdateType.InlineQuery => update.InlineQuery?.Query,
            UpdateType.PollAnswer => update.PollAnswer?.PollId,
            UpdateType.ShippingQuery => update.ShippingQuery?.InvoicePayload,
            UpdateType.ChatJoinRequest => update.ChatJoinRequest?.Bio,
            UpdateType.ChosenInlineResult => update.ChosenInlineResult?.Query,
            UpdateType.EditedChannelPost => $"{update.EditedChannelPost?.Caption} \n {update.EditedChannelPost?.Text}",
            UpdateType.MyChatMember => $"{update.MyChatMember?.From?.Id} : {update.MyChatMember?.From?.Username}",
            UpdateType.PreCheckoutQuery => update.PreCheckoutQuery?.InvoicePayload,
            UpdateType.Unknown => null,
        };

        await _botRepository.AddUpdate(saveUpdateDto);
    }
}