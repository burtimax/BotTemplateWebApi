using System.Reflection;
using Mapster;
using MapsterMapper;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Utils;
using MultipleTestBot.Endpoints.User.GetUsers.RequestResponse;
using MultipleTestBot.Repository.Bot;
using MultipleTestBot.Repository.User;
using MultipleTestBot.Resources;

namespace MultipleTestBot.Extensions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Зарегистрировать класс ресурсов бота. 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="resourcesFilePath"></param>
    /// <returns></returns>
    public static BotResources ConfigureBotResources(this IServiceCollection services, string resourcesFilePath)
    {
        if (resourcesFilePath == null) throw new ArgumentNullException(nameof(resourcesFilePath));
        
        string json = File.ReadAllText(resourcesFilePath);
        BotResourcesBuilder resourcesBuilder = new(json);
        json = resourcesBuilder.Build();

        Stream jsonStream = StreamHelper.GenerateStreamFromString(json);
        var resourcesConfigBuilder = new ConfigurationBuilder().AddJsonStream(jsonStream);
        IConfiguration resourcesConfiguration = resourcesConfigBuilder.Build();
        
        services.Configure<BotResources>(resourcesConfiguration);
        return resourcesConfiguration.Get<BotResources>();
    }
    
    /// <summary>
    /// Добавить маппер в DI.
    /// </summary>
    /// <param name="services"></param>
    public static void AddMapster(this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(
            Assembly.GetExecutingAssembly());
        config.RequireExplicitMapping = false;
        config.RequireDestinationMemberSource = false;

        config.When((srcType, destType, _) => true)
            .IgnoreNullValues(true);
        
        config
            .When((srcType, destType, _) => srcType == typeof(IBaseBotEntityWithoutIdentity) == false && destType == typeof(IBaseBotEntityWithoutIdentity))
            .Ignore("Id",
                nameof(IBaseBotEntityWithoutIdentity.CreatedAt),
                nameof(IBaseBotEntityWithoutIdentity.UpdatedAt),
                nameof(IBaseBotEntityWithoutIdentity.DeletedAt));
        
        var mapperConfig = new Mapper(config);
        services.AddSingleton<IMapper>(mapperConfig);
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IBotRepository, BotRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}