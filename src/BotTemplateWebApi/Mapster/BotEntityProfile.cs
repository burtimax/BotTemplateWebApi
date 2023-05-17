using BotFramework.Db.Entity;
using Mapster;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.Mapster;

/// <summary>
/// Профили маппинга сущностей бота.
/// </summary>
public class BotEntityProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, BotUser>()
            .Map(d => d.TelegramId, s => s.Id)
            .Map(d => d.TelegramUsername, s => s.Username)
            .Map(d => d.TelegramFirstname, s => s.FirstName)
            .Map(d => d.TelegramLastname, s => s.LastName)
            .Ignore(d => d.Phone)
            .Ignore(d => d.Role)
            .Ignore(d => d.Status)
            .Ignore(d => d.Id)
            .Ignore(d => d.CreatedAt)
            .Ignore(d => d.UpdatedAt)
            .Ignore(d => d.DeletedAt);
    }
}