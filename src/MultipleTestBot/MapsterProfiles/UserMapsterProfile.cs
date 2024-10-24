using Mapster;
using MultipleBotFramework.Db.Entity;
using MultipleTestBot.Endpoints.User.GetUsers.RequestResponse;

namespace MultipleTestBot.MapsterProfiles;

public class UserMapsterProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<BotUserEntity, UserResponse>()
            .Map(d => d.TelegramUsername, s => s.GetUsernameLink());
    }
}