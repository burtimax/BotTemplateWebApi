using System.Reflection;
using BotFramework.Options;
using BotFramework.Other;
using BotTemplateWebApi.Resources;
using Mapster;
using MapsterMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;

namespace BotTemplateWebApi.Extentsions;

public static class IServiceCollectionExtensions
{
    /// <summary>
    /// Расширения для регистрации конфигурации и сопоставления Mapster в коллекции служб.
    /// </summary>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="config">Конфигурация для Mapster.</param>
    public static void AddMapster(this IServiceCollection services, TypeAdapterConfig config)
    {
        var mapperConfig = new Mapper(config);
        services.AddSingleton<IMapper>(mapperConfig);
    }

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

        // var resourcesConfigBuilder = new ConfigurationBuilder().AddJsonFile(resourcesFilePath, false, true);
        // IConfiguration resourcesConfiguration = resourcesConfigBuilder.Build();

        Stream jsonStream = GenerateStreamFromString(json);
        var resourcesConfigBuilder = new ConfigurationBuilder().AddJsonStream(jsonStream);
        IConfiguration resourcesConfiguration = resourcesConfigBuilder.Build();
        
        services.Configure<BotResources>(resourcesConfiguration);
        return resourcesConfiguration.Get<BotResources>();
    }
    
    private static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
    
    /// <summary>
    /// Расширения для регистрации конфигурации и сопоставления Mapster в коллекции служб.
    /// </summary>
    /// <remarks>
    /// Используется кастомный <see cref="TypeAdapterConfig"/>, вы можете настроить свою конфигурации и использовать <see cref="AddMapster(IServiceCollection, TypeAdapterConfig)"/>.
    /// </remarks>
    /// <param name="services">Коллекция служб.</param>
    /// <param name="assemblies">Сборки (сборка самого шлюза, + опционально сборки Kit библиотек).</param>
    public static void AddMapster(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            throw new NullReferenceException("Assemblies cannot be null");

        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(assemblies);
        config.RequireExplicitMapping = true;
        config.RequireDestinationMemberSource = true;
        config
            .When((srcType, destType, _) => srcType == destType)
            .Ignore("Id");
        
        services.AddMapster(config);
    }
}