using System.Reflection;
using Mapster;
using MapsterMapper;

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