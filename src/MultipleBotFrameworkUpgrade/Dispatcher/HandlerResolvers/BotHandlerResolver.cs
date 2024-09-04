using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MultipleBotFrameworkUpgrade.Attributes;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Dto;
using MultipleBotFrameworkUpgrade.Enums;

namespace MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;

public class BotHandlerResolver
{ 
    private readonly Assembly[] _assemblies;
    private static Type _handlerType = typeof(IBaseBotHandler);
    private static Type _attributeType = typeof(BotHandlerAttribute);

    public BotHandlerResolver(params Assembly[] assemblies)
    {
        if (assemblies is null) throw new ArgumentNullException(nameof(assemblies));
        
        _assemblies = assemblies;
    }

    /// <summary>
    /// Получить тип приоритетного обработчика состояния чата.
    /// </summary>
    /// <param name="command">Наименование состояния чата.</param>
    /// <returns></returns>
    public List<Type>? GetHandlers(BotHandlerResolverArgs args)
    {
        if (_assemblies is null || _assemblies.Any() == false) throw new Exception("Нет данные по сборкам");
        
        // Получаем все типы всех обработчиков бота из всех сборок.
        List<Type>? handlerTypes = _assemblies?.SelectMany(a => a.GetTypes())
            ?.Where(t => _handlerType.IsAssignableFrom(t))
            ?.ToList();

        if (handlerTypes is null || handlerTypes.Any() == false) return null;

        // Отфильтровываем обработчики.
        handlerTypes = FilterHandlers(handlerTypes, args);

        if (handlerTypes is null || handlerTypes.Any() == false) return null;
        
        SortHandlers(handlerTypes, args);
        return handlerTypes;
    }


    /// <summary>
    /// Отсеиваем обработчики, которые явно не подходят.
    /// </summary>
    /// <returns></returns>
    private List<Type>? FilterHandlers(List<Type> handlers, BotHandlerResolverArgs args)
    {
        if (handlers is null || handlers.Any() == false) return null;
        
        List<Type> filteredHandlers = new();
        foreach (var handler in handlers)
        {
            List<BotHandlerAttribute>? attrs = GetRequiredHandlerAttributes(handler);
            if(attrs is null || attrs.Any() == false) continue;

            if(HasApproproateAttribute(attrs, args))
                filteredHandlers.Add(handler);
        }

        return filteredHandlers;
    }

    private List<BotHandlerAttribute>? GetRequiredHandlerAttributes(Type handler) =>
        Attribute.GetCustomAttributes(handler, _attributeType)
            ?.Where(attr => attr is not null)
            ?.Select(attr => attr as BotHandlerAttribute)
            ?.ToList();

    /// <summary>
    /// Проверяем, есть ли среди атрибутов тот, который удовлетворит условиям аргументов.
    /// Есть ли подходящий атрибут для условий.
    /// </summary>
    /// <param name="attrs"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private bool HasApproproateAttribute(List<BotHandlerAttribute> attrs, BotHandlerResolverArgs args)
    {
        bool IsAppropriate1(string? arg, string? attrValue) => string.IsNullOrEmpty(attrValue) || string.Equals(attrValue ?? "", arg ?? "");

        bool IsAppropriate2<T>(T arg, IEnumerable<T>? attrValues) => (attrValues is null || attrValues.Any() == false) || (arg is not null && attrValues.Contains(arg));
        bool IsAppropriate3<T>(IEnumerable<T>? argValues, IEnumerable<T>? attrValues) => (attrValues is null || attrValues.Any() == false) || 
            (argValues is not null && argValues.Any() && attrValues.Intersect(argValues).Count() > 0);
        
        foreach (var attr in attrs)
        {
            if (IsAppropriate1(args.StateName, attr.StateName) == false) continue;
            
            if ((string.IsNullOrEmpty(attr.Command) || (args.Command ?? "").StartsWith(attr.Command)) == false) continue;

            if (IsAppropriate2(args.UpdateType, attr.UpdateTypes) == false) continue;
            
            if (IsAppropriate2(args.UserRole, attr.UserRoles) == false) continue;

            if (IsAppropriate2(args.BotId, attr.BotIds) == false) continue;

            if (IsAppropriate3(args.UserClaims?.Select(c => c.Name), attr.UserClaims) == false) break;

            return true;
        }

        return false;
    }

    private void SortHandlers(List<Type> handlers, BotHandlerResolverArgs args)
    {
        handlers.Sort((t1, t2) =>
        {
            double differ = CountHandlerFactor(t2,args) - CountHandlerFactor(t1, args);

            return differ switch
            {
                < 0 => -1,
                > 0 => 1,
                _ => 0
            };
        });
    }

    /// <summary>
    /// Сделать количественный расчет точности обработчика аргументам.
    /// Чем больше значение, тем вероятнее что обработчик получит запрос от бота.
    /// Рассчитывается максимальное соответствие аргументам.
    /// </summary>
    /// <param name="handler"></param>
    /// <returns></returns>
    private int CountHandlerFactor(Type handler, BotHandlerResolverArgs args)
    {
        int max = int.MinValue;
        List<BotHandlerAttribute>? attrs = GetRequiredHandlerAttributes(handler);
        if (attrs is null || attrs.Any() == false) return max;

        int versionFactor = 10;
        int commandFactor = 10000 * versionFactor;
        int botIdFactor = 105 * versionFactor;
        int updateTypeFactor = 104 * versionFactor;
        int stateNameFactor = 100 * versionFactor;
        int userRoleFactor = 100 * versionFactor;
        int userClaimsFactor = 100 * versionFactor;
        
        foreach (var attr in attrs)
        {
            int sum = 0;

            if (args.BotId > 0
                && attr.BotIds is not null && attr.BotIds.Any()
                && attr.BotIds.Contains(args.BotId)) sum += botIdFactor;
            
            if (string.IsNullOrEmpty(args.Command) == false
                && string.IsNullOrEmpty(attr.Command) == false
                && args.Command.StartsWith(attr.Command)) sum += commandFactor;
            
            if (string.IsNullOrEmpty(args.StateName) == false
                && string.IsNullOrEmpty(attr.StateName) == false
                && string.Equals(args.StateName, attr.StateName)) sum += stateNameFactor;

            if (attr.UpdateTypes is not null 
                && attr.UpdateTypes.Any()
                && attr.UpdateTypes.Contains(args.UpdateType)) sum += updateTypeFactor;

            if (attr.UserRoles is not null 
                && attr.UserRoles.Any()
                && string.IsNullOrEmpty(args.UserRole) == false
                && attr.UserRoles.Contains(args.UserRole)) sum += userRoleFactor;

            if (args.UserClaims is not null
                && args.UserClaims.Any()
                && attr.UserClaims is not null
                && attr.UserClaims.Any()
                && attr.UserClaims.Intersect(args.UserClaims.Select(c => c.Name)).Count() > 0)
            {
                sum += userClaimsFactor;
            }

            sum += (int)attr.Version * versionFactor;

            max = sum > max ? sum : max;
        }

        return max;
    }
}