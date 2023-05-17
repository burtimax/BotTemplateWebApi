using BotTemplateWebApi.Extentsions;
using Mapster;

namespace BotTemplate.Tests.Mapping;

public class MapsterTests
{
    [Fact(DisplayName = "[Mapster] Проверка профилей маппинга.")]
    public void CheckMappingProfiles()
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(config.GetBotWebApiAssembly());
        config.RequireExplicitMapping = true;
        config.RequireDestinationMemberSource = true;
        config
            .When((srcType, destType, _) => srcType == destType)
            .Ignore("Id");
        // config.Default.UseDestinationValue(member =>
        //     member is { SetterModifier: AccessModifier.None, Type.IsGenericType: true } &&
        //     member.Type.GetGenericTypeDefinition() == typeof(RepeatedField<>));
        config.Compile();
    }
}