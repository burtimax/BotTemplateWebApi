using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;

namespace MultipleBotFramework.Dispatcher.HandlerResolvers;

public class BotStateHandlerResolver
{
    private readonly Assembly[] _assemblies;
    private readonly Type _baseBotStateType = typeof(BaseBotState);

    public BotStateHandlerResolver(params Assembly[] assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));
        
        _assemblies = assemblies;
    }

    /// <summary>
    /// Получить тип приоритетного обработчика состояния чата.
    /// </summary>
    /// <param name="stateName">Наименование состояния чата.</param>
    /// <returns></returns>
    public Type? GetPriorityStateHandlerType(string stateName, string userRole)
    {
        List<Type> allTypes = new List<Type>();

        foreach (var assembly in _assemblies)
        {
            allTypes.AddRange(assembly.GetTypes());
        }
        
        IEnumerable<Type>? handlerTypes = FilterBotHandlerTypes(allTypes);

        if (handlerTypes.Any() == false)
        {
            return null;
        }
        
        IEnumerable<Type> stateHandlerTypes = handlerTypes.Where(t => IsStateHandler(t, stateName));

        Type? handlerType = GetHandlerTypeWithHighestPriority(stateHandlerTypes, stateName, userRole);

        return handlerType;
    }

    /// <summary>
    /// Среди переданных типов обработчиков получить тип с наибольшим приоритетом.
    /// </summary>
    /// <param name="handlerTypes">Список типов обработчиков.</param>
    /// <returns>Наиболее приоритетный обработчик.</returns>
    private Type GetHandlerTypeWithHighestPriority(IEnumerable<Type> types, string stateName, string userRole)
    {
        if (types == null || types.Any() == false)
        {
            throw new ArgumentNullException(nameof(types));
        }

        IEnumerable<Type> handlerTypes = FilterBotHandlerTypes(types);
        
        List<Type> handlersWithBotStateAttribute = handlerTypes
            .Where(t => IsStateHandler(t, stateName))
            ?.ToList();

        // Если нет обработчиков с атрибутами, тогда вернуть любой.
        if (handlersWithBotStateAttribute == null || handlersWithBotStateAttribute.Any() == false)
        {
            return types.First();
        }
        
        FilterStateHandlersForUserRole(userRole, ref handlersWithBotStateAttribute);

        handlersWithBotStateAttribute.Sort((t1, t2) =>
        {
            double differ = GetHighestVersion(t2, stateName) - GetHighestVersion(t1, stateName);

            return differ switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        });

        return handlersWithBotStateAttribute.First();
    }

    /// <summary>
    /// Фильтруем обработчики для пользователя.
    /// </summary>
    /// <param name="stateHandlers"></param>
    void FilterStateHandlersForUserRole(string userRole, ref List<Type> stateHandlers)
    {
        List<Type> userHandlers = new();

        foreach (var type in stateHandlers)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof(BotStateAttribute));
        
            bool hasUserRoleAttr = attributes.Any(attr =>
            {
                if (attr is BotStateAttribute attribute 
                    && string.Equals(attribute.UserRole,userRole, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            });

            if (hasUserRoleAttr)
            {
                userHandlers.Add(type);
            }
        }

        if (userHandlers != null && userHandlers.Any())
        {
            stateHandlers = userHandlers;
        }
    }

    /// <summary>
    /// inline метод получения наибольшей версии 
    /// </summary>
    /// <param name="t">Тип обработчика</param>
    /// <returns></returns>
    private double GetHighestVersion(Type t, string stateName)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(BotStateAttribute));

        // Не забываем отфильтровать атрибуты по наименованию состояния,
        // чтобы не брать версии из атрибутов других состояний.
        return attributes
            .Where(attr => IsStateAttribute((attr as BotStateAttribute).StateName,stateName))
            .Max(attr => (attr as BotStateAttribute).Version);
    }
    
    /// <summary>
    /// Проверяем является текущий тип обработчиком состояния с определенным наименованием.
    /// Не учитываем регистр наименования состояния.
    /// </summary>
    /// <param name="handlerType">Тип обработчика.</param>
    /// <param name="stateName">Наименование состояния.</param>
    /// <returns></returns>
    private bool IsStateHandler(Type handlerType, string stateName)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(handlerType, typeof(BotStateAttribute));
        
        return attributes.Any(attr =>
        {
            if (attr is BotStateAttribute attribute 
                && IsStateAttribute(attribute.StateName, stateName))
            {
                return true;
            }

            return false;
        });
    }

    private bool IsStateAttribute(string attributeName, string stateName)
    {
        return string.Equals(attributeName, stateName, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Получить из списка типов только те, что являются обработчиками состояния бота.
    /// </summary>
    /// <param name="types">Список типов.</param>
    /// <returns></returns>
    private IEnumerable<Type>? FilterBotHandlerTypes(IEnumerable<Type> types)
    {
        return types
            ?.Where(t => _baseBotStateType.IsAssignableFrom(t))
            ?.ToList() ?? null;
    }
}