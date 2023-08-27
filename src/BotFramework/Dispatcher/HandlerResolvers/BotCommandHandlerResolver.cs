using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Attributes;
using BotFramework.Base;

namespace BotFramework.Dispatcher.HandlerResolvers;

public class BotCommandHandlerResolver
{ private readonly Assembly _assembly;
    private readonly Type _baseBotCommandType = typeof(BaseBotCommand);

    public BotCommandHandlerResolver(Assembly assembly)
    {
        if (assembly is null) throw new ArgumentNullException(nameof(assembly));
        
        _assembly = assembly;
    }

    /// <summary>
    /// Получить тип приоритетного обработчика состояния чата.
    /// </summary>
    /// <param name="command">Наименование состояния чата.</param>
    /// <returns></returns>
    public Type? GetPriorityCommandHandlerType(string command, string userRole)
    {
        IEnumerable<Type>? handlerTypes = FilterBotCommandTypes(_assembly.GetTypes());

        if (handlerTypes.Any() == false)
        {
            return null;
        }
        
        IEnumerable<Type> commandHandlerTypes = handlerTypes.Where(t => IsCommandHandler(t, command));

        Type? handlerType = GetHandlerTypeWithHighestPriority(commandHandlerTypes, command, userRole);

        return handlerType;
    }

    /// <summary>
    /// Среди переданных типов обработчиков команд получить тип с наибольшим приоритетом.
    /// </summary>
    /// <param name="handlerTypes">Список типов обработчиков команд.</param>
    /// <returns>Наиболее приоритетный обработчик.</returns>
    private Type GetHandlerTypeWithHighestPriority(IEnumerable<Type> types, string command, string userRole)
    {
        if (types == null || types.Any() == false)
        {
            throw new ArgumentNullException(nameof(types));
        }

        IEnumerable<Type> handlerTypes = FilterBotCommandTypes(types);
        
        List<Type> handlersWithBotCommandAttribute = handlerTypes
            .Where(t => IsCommandHandler(t, command))
            ?.ToList();

        // Если нет обработчиков с атрибутами, тогда вернуть любой.
        if (handlersWithBotCommandAttribute == null || handlersWithBotCommandAttribute.Any() == false)
        {
            return types.First();
        }
        
        FilterStateHandlersForUserRole(userRole, ref handlersWithBotCommandAttribute);

        handlersWithBotCommandAttribute.Sort((t1, t2) =>
        {
            double differ = GetHighestVersion(t2, command) - GetHighestVersion(t1, command);

            return differ switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        });

        return handlersWithBotCommandAttribute.First();
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
            Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof(BotCommandAttribute));
        
            bool hasUserRoleAttr = attributes.Any(attr =>
            {
                if (attr is BotCommandAttribute attribute 
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
    /// inline метод получения наибольшей версии. 
    /// </summary>
    /// <param name="t">Тип обработчика</param>
    /// <returns></returns>
    private double GetHighestVersion(Type t, string command)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(BotCommandAttribute));

        // Не забываем отфильтровать атрибуты по вхождению строки команды,
        // чтобы не брать версии из атрибутов других команд.
        return attributes
            .Where(attr => IsCommandAttribute((attr as BotCommandAttribute).Command,command))
            .Max(attr => (attr as BotCommandAttribute).Version);
    }
    
    /// <summary>
    /// Проверяем является текущий тип обработчиком команды с определенным наименованием.
    /// Не учитываем регистр строки команды.
    /// </summary>
    /// <param name="handlerType">Тип обработчика.</param>
    /// <param name="command">Команда от пользователя.</param>
    /// <returns></returns>
    private bool IsCommandHandler(Type handlerType, string command)
    {
        Attribute[] attributes = Attribute.GetCustomAttributes(handlerType, typeof(BotCommandAttribute));
        
        return attributes.Any(attr =>
        {
            if (attr is BotCommandAttribute attribute
                && IsCommandAttribute(attribute.Command, command))
            {
                return true;
            }

            return false;
        });
    }

    private bool IsCommandAttribute(string attributeName, string command)
    {
        return command.StartsWith(attributeName, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Получить из списка типов только те, что являются обработчиками команд бота.
    /// </summary>
    /// <param name="types">Список типов.</param>
    /// <returns></returns>
    private IEnumerable<Type>? FilterBotCommandTypes(IEnumerable<Type> types)
    {
        return types
            ?.Where(t => _baseBotCommandType.IsAssignableFrom(t))
            ?.ToList() ?? null;
    }
}