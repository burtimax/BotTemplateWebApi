using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultipleBotFramework.Attributes;
using MultipleBotFramework.Base;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dto;

namespace MultipleBotFramework.Dispatcher.HandlerResolvers;

public class BotCommandHandlerResolver
{ private readonly Assembly[] _assemblies;
    private readonly Type _baseBotCommandType = typeof(BaseBotCommand);

    public BotCommandHandlerResolver(params Assembly[] assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));
        
        _assemblies = assemblies;
    }

    /// <summary>
    /// Получить тип приоритетного обработчика состояния чата.
    /// </summary>
    /// <param name="command">Наименование состояния чата.</param>
    /// <returns></returns>
    public Type? GetPriorityCommandHandlerType(string command, BotUserEntity userEntity, IEnumerable<ClaimValue>? userClaims)
    {
        List<Type>? assembliesTypes = new List<Type>();

        foreach (var assembly in _assemblies)
        {
            assembliesTypes.AddRange(assembly.GetTypes());
        }
        
        IEnumerable<Type>? handlerTypes = FilterBotCommandTypes(assembliesTypes);

        if (handlerTypes.Any() == false)
        {
            return null;
        }
        
        IEnumerable<Type> commandHandlerTypes = handlerTypes.Where(t => IsCommandHandler(t, command, userEntity));

        if (commandHandlerTypes == null || commandHandlerTypes.Any() == false) return null;
        
        Type? handlerType = GetHandlerTypeWithHighestPriority(commandHandlerTypes, command, userEntity, userClaims);

        return handlerType;
    }

    /// <summary>
    /// Среди переданных типов обработчиков команд получить тип с наибольшим приоритетом.
    /// </summary>
    /// <param name="handlerTypes">Список типов обработчиков команд.</param>
    /// <returns>Наиболее приоритетный обработчик.</returns>
    private Type? GetHandlerTypeWithHighestPriority(IEnumerable<Type> types, string command, BotUserEntity userEntity, IEnumerable<ClaimValue>? userClaims)
    {
        if (types == null || types.Any() == false)
        {
            throw new ArgumentNullException(nameof(types));
        }

        IEnumerable<Type> handlerTypes = FilterBotCommandTypes(types);
        
        List<Type> handlersWithBotCommandAttribute = handlerTypes
            .Where(t => IsCommandHandler(t, command, userEntity))
            ?.ToList();

        // Если нет обработчиков с атрибутами, тогда вернуть любой.
        if (handlersWithBotCommandAttribute == null || handlersWithBotCommandAttribute.Any() == false)
        {
            return handlerTypes.FirstOrDefault();
        }
        
        FilterStateHandlersForUser(userEntity, userClaims, ref handlersWithBotCommandAttribute);

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

        return handlersWithBotCommandAttribute.FirstOrDefault();
    }

    /// <summary>
    /// Фильтруем обработчики для пользователя.
    /// </summary>
    /// <param name="stateHandlers"></param>
    void FilterStateHandlersForUser(BotUserEntity userEntity, IEnumerable<ClaimValue>? userClaims, ref List<Type> stateHandlers)
    {
        List<Type> userHandlers = new();

        /// Фильтруем обработчики для пользователя.
        foreach (var type in stateHandlers)
        {
            Attribute[] attributes = Attribute.GetCustomAttributes(type, typeof(BotCommandAttribute));
        
            // Проверка наличия у пользователя роли для команды
            bool hasUserRoleAttr = attributes.All(attr =>
            {
                if (attr is BotCommandAttribute attribute 
                    && (string.IsNullOrEmpty(attribute.RequiredUserRole) || string.Equals(attribute.RequiredUserRole,userEntity.Role, StringComparison.OrdinalIgnoreCase)))
                {
                    return true;
                }

                return false;
            });
            
            // Проверяем что у пользователя есть все требуемые разрешения для этого обработчика
            bool hasUserClaimsAttr = attributes.All(attr =>
            {
                if (attr is BotCommandAttribute attribute)
                {
                    if (attribute.RequiredUserClaims == null) return true;
                    
                    foreach (var claim in attribute.RequiredUserClaims)
                    {
                        if (userClaims == null || userClaims.Select(c => c.Name).Contains(claim) == false)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }

                return true;
            });

            if (hasUserRoleAttr && hasUserClaimsAttr)
            {
                userHandlers.Add(type);
            }
        }

        stateHandlers = userHandlers ?? new List<Type>();
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
    private bool IsCommandHandler(Type handlerType, string command, BotUserEntity userEntity)
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