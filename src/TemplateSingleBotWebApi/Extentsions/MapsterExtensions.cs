using System.Reflection;
using Mapster;

namespace BotTemplateWebApi.Extentsions;

public static class MapsterExtensions
{
    /// <summary>
    /// Получить сборку проекта.
    /// </summary>
    /// <param name="config">Конфигуратор Мапстера.</param>
    /// <returns></returns>
    public static Assembly GetBotWebApiAssembly(this TypeAdapterConfig config) 
        => Assembly.GetExecutingAssembly();
}