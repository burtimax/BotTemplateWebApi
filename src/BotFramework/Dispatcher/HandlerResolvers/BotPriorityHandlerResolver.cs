using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Dispatcher.HandlerResolvers;

public class BotPriorityHandlerResolver
{
    private readonly Assembly[] _assemblies;
    private readonly Type _baseHandlerType = typeof(BaseBotPriorityHandler);

    public BotPriorityHandlerResolver(params Assembly[] assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));
        
        _assemblies = assemblies;
    }

    /// <summary>
    /// Получить тип приоритетного обработчика типа запроса.
    /// </summary>
    /// <returns></returns>
    public Type? GetPriorityTypeHandler(UpdateType updateType)
    {
        List<Type>? assembliesTypes = new List<Type>();

        foreach (var assembly in _assemblies)
        {
            assembliesTypes.AddRange(assembly.GetTypes());
        }
        
        IEnumerable<Type>? handlerTypes = GetChildrenOfBaseType(assembliesTypes);

        if (handlerTypes.Any() == false)
        {
            return null;
        }
        
        IEnumerable<Type> handlers = handlerTypes.Where(t => HasSpecifiedAttribute(t, updateType));

        if (handlers == null || handlers.Any() == false) return null;
        
        Type? handlerType = GetHandlerTypeWithHighestPriority(handlers, updateType);

        return handlerType;
    }

    /// <summary>
    /// Среди переданных обработчиков получить с наибольшим приоритетом.
    /// </summary>
    /// <param name="handlerTypes">Список типов обработчиков запроса.</param>
    /// <returns>Наиболее приоритетный обработчик.</returns>
    private Type? GetHandlerTypeWithHighestPriority(IEnumerable<Type> types, UpdateType updateType)
    {
        if (types == null || types.Any() == false)
        {
            throw new ArgumentNullException(nameof(types));
        }

        IEnumerable<Type> handlerTypes = GetChildrenOfBaseType(types);
        
        List<Type> handlersWithBotSSpecifiedAttribute = handlerTypes
            .Where(t => HasSpecifiedAttribute(t, updateType))
            ?.ToList();

        // Если нет обработчиков с атрибутами, тогда вернуть любой.
        if (handlersWithBotSSpecifiedAttribute == null || handlersWithBotSSpecifiedAttribute.Any() == false)
        {
            return handlerTypes.FirstOrDefault();
        }

        handlersWithBotSSpecifiedAttribute.Sort((t1, t2) =>
        {
            double differ = GetHighestVersion(t2, updateType) - GetHighestVersion(t1, updateType);

            return differ switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        });

        return handlersWithBotSSpecifiedAttribute.FirstOrDefault();
    }

    /// <summary>
    /// inline метод получения наибольшей версии. 
    /// </summary>
    /// <param name="t">Тип обработчика</param>
    /// <returns></returns>
    private double GetHighestVersion(Type t, UpdateType updateType)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(BotPriorityHandlerAttribute));

        // Не забываем отфильтровать атрибуты по вхождению строки команды,
        // чтобы не брать версии из атрибутов других команд.
        return attributes
            .Where(attr => (attr as BotPriorityHandlerAttribute).UpdateType == updateType)
            .Max(attr => (attr as BotPriorityHandlerAttribute).Version);
    }
    
    /// <summary>
    /// Проверяем есть ли у обработчика специальный атрибут. 
    /// </summary>
    /// <param name="handlerType">Тип обработчика.</param>
    /// <param name="updateType">Тип запроса от пользователя.</param>
    /// <returns></returns>
    private bool HasSpecifiedAttribute(Type handlerType, UpdateType updateType)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(handlerType, typeof(BotPriorityHandlerAttribute));
        
        return attributes.Any(attr =>
        {
            if (attr is BotPriorityHandlerAttribute attribute
                && attribute.UpdateType == updateType)
            {
                return true;
            }

            return false;
        });
    }
    
    /// <summary>
    /// Получить из списка типов только те, что являются обработчиками типа запроса.
    /// </summary>
    /// <param name="types">Список типов.</param>
    /// <returns></returns>
    private IEnumerable<Type>? GetChildrenOfBaseType(IEnumerable<Type> types)
    {
        return types
            ?.Where(t => _baseHandlerType.IsAssignableFrom(t))
            ?.ToList() ?? null;
    }
}