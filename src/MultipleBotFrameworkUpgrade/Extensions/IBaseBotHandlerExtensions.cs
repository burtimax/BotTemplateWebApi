using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using MultipleBotFrameworkUpgrade.Base;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Dispatcher.HandlerResolvers;
using MultipleBotFrameworkUpgrade.Dto;
using MultipleBotFrameworkUpgrade.Enums;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFrameworkUpgrade.Extensions;

public static class IBaseBotHandlerExtensions
{
    /// <summary>
    /// Создать экзмпляр обработчика и вернуть.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T GetHandlerInstance<T>(this IBaseBotHandler handler) where T : IBaseBotHandler
    {
        // T stateInstance = (T) ActivatorUtilities.CreateInstance(handler.ServiceProvider, typeof(T), new object[] {handler.ServiceProvider });
        //
        // if (stateInstance == null)
        // {
        //     throw new Exception($"Не могу создать экземпляр типа [{typeof(T).FullName}]");
        // }
        //
        // // Инициализируем свойства класса базового состояния бота. 
        // stateInstance.Chat = handler.Chat;
        // stateInstance.User = handler.User;
        // stateInstance.Update = handler.Update;
        // stateInstance.BotId = handler.BotId;
        // stateInstance.BotClient = handler.BotClient;
        // stateInstance.IsOwner = handler.IsOwner;
        // stateInstance.ServiceProvider = handler.ServiceProvider;
        // stateInstance.UserClaims = handler.UserClaims?.ToList()?.AsReadOnly() 
        //                            ?? new List<ClaimValue>().AsReadOnly();
        //
        // return stateInstance;
        return (T) GetHandlerInstance(handler, typeof(T));
    }
    
    public static IBaseBotHandler GetHandlerInstance(this IBaseBotHandler handler, Type handlerType)
    {
        IBaseBotHandler stateInstance = (IBaseBotHandler) ActivatorUtilities.CreateInstance(handler.ServiceProvider, handlerType, new object[] {handler.ServiceProvider });
        
        if (stateInstance == null)
        {
            throw new Exception($"Не могу создать экземпляр типа [{handlerType.FullName}]");
        }
        
        // Инициализируем свойства класса базового состояния бота. 
        stateInstance.Chat = handler.Chat;
        stateInstance.User = handler.User;
        stateInstance.Update = handler.Update;
        stateInstance.BotId = handler.BotId;
        stateInstance.BotClient = handler.BotClient;
        stateInstance.IsOwner = handler.IsOwner;
        stateInstance.ServiceProvider = handler.ServiceProvider;
        stateInstance.UserClaims = handler.UserClaims?.ToList()?.AsReadOnly() 
                                   ?? new List<ClaimValue>().AsReadOnly();

        return stateInstance;
    }
    
    public static IBaseBotHandler ResolveHandlers(this IBaseBotHandler handler)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        BotHandlerResolver handlerResolver = new(assemblies);
        BotHandlerResolverArgs resolverArgs = new()
        {
            BotId = handler.BotId,
            UserRole = handler.User?.Role,
            User = handler.User,
            UserClaims = handler.UserClaims,
            UpdateType = handler.Update.Type(),
            StateName = handler?.Chat?.States?.CurrentState ?? BotConstants.StartState,
            Command = handler.Update.Type() == UpdateType.Command ? handler.Update!.Message!.Text : null,
        };
        
        List<Type>? handlerTypes = handlerResolver.GetHandlers(resolverArgs);
        if (handlerTypes is null || handlerTypes.Any() == false) return null;
        return GetHandlerInstance(handler, handlerTypes.First());
    }
}