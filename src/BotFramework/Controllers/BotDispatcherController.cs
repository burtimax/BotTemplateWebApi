using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Attributes;
using BotFramework.Db.Entity;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using BotFramework.Filters;
using BotFramework.Other;
using BotFramework.Repository;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BotFramework.Controllers;

[Controller]
public class BotDispatcherController : BaseBotController
{
    private static int t = 0;
    
    private readonly IMapper _mapper;
    private readonly IBaseBotRepository _botRepository;
    private readonly HttpContext _context;
    private readonly Assembly _assembly;
    
    public BotDispatcherController(IMapper mapper, IBaseBotRepository botRepository,
        IHttpContextAccessor contextAccessor,
        Assembly assembly)
    {
        _assembly = assembly;
        _mapper = mapper;
        _botRepository = botRepository;
        _context = contextAccessor.HttpContext;
    }
    
    //[HttpGet("/")]
    public override async Task<IActionResult> HandleBotRequest(Update updateRequest)
    {
        // HttpRequest request = _context.Request;
        // Update? update = null;
        //
        // update = updateRequest; //await GetUpdateFromRequest(request);
        //
        // if (update == null) throw new NullUpdateModelInMiddleWareException();
        //
        // User telegramUser = update.GetUser();
        // Chat telegramChat = update.GetChat();
        //
        // // Сохраняем или обновляем информацию о пользователе.
        // BotUser user = await _botRepository.UpsertUser(telegramUser);
        //
        // // Сохраняем чат, если еще не существует. 
        // BotChat? existedChat = await _botRepository.GetChat(telegramChat.Id);
        // BotChat chat = existedChat ?? await _botRepository.AddChat(telegramChat, user);
        //
        // // Получаем текушее состояние чата. 
        // string currentState = chat.States.CurrentState;
        //
        // // Собираем всю необходимую информацию для дальнейшей обработки состояний 
        // // Получили состояние чата, теперь меняем строку запроса и переходим в новый контроллер уже дальше по pipeline.
        //
        // // Перенаправляем запрос на сервисы

        
        try
        {
            BotHandlerResolver resolver = new(_assembly);
            Type? handlerType = resolver.GetPriorityStateHandlerTypeByStateName("Test");
            
            return await ProcessRequestByHandler(handlerType.FullName /*"BotTemplateWebApi.States.TestBot.TestBotState"*/, updateRequest) ;
        }
        catch (Exception e)
        {
            return Ok();
        }
       
        return Ok();

        // Отправляем ответ пользователю
    }


    // private string? GetTypeNameByChatState(string chatState)
    // {
    //     Type baseBotState = typeof(BaseBotState);
    //     
    //     IEnumerable<Type> stateHandlerTypes = _assembly.GetTypes().Where(t => baseBotState.IsAssignableFrom(t));
    //     
    //     IEnumerable<Type> HandlerTypes = stateHandlerTypes.Where(t =>
    //     {
    //         Attribute[] attributes = Attribute.GetCustomAttributes(t, typeof(BotStateAttribute));
    //         return attributes.Any(attr =>
    //         {
    //             BotStateAttribute a = attr as BotStateAttribute;
    //             if (string.Equals(a.StateName, chatState))
    //             {
    //                 return true;
    //             }
    //
    //             return false;
    //         });
    //     });
    //     Type? handlerType = HandlerTypes.FirstOrDefault();
    //
    //     return handlerType?.FullName;
    //
    // }

    /// <summary>
    /// Запустить обработчик типа состояния бота и получить результат. 
    /// </summary>
    /// <param name="handlerTypeName">Наименование типа обработчика.</param>
    /// <param name="update">Объект запроса от Telegram API по webhook.</param>
    /// <returns></returns>
    /// <exception cref="NotFoundHandlerTypeException">Не найден тип обработчика запроса.</exception>
    /// <exception cref="NotFoundHandlerMethodException">Не найден метод обработчика запроса.</exception>
    private Task<IActionResult> ProcessRequestByHandler (string handlerTypeName, Update update)
    {
        Type? handlerType = _assembly.GetType(handlerTypeName, true, true);

        if (handlerType == null)
        {
            throw new NotFoundHandlerTypeException(handlerTypeName, _assembly.GetName().Name);
        }
        
        object? handlerInstance = _assembly.CreateInstance(handlerTypeName, true, BindingFlags.Default, null, new object[]{ HttpContext.RequestServices}, null, null);

        // Каждое состояние должно быть наследником типа BaseBotState и реализовывать метод HandleBotRequest
        string handlerMethodName = nameof(BaseBotState.HandleBotRequest);
        
        MethodInfo? handler = handlerType?.GetMethod(handlerMethodName);

        if (handler == null)
        {
            throw new NotFoundHandlerMethodException(handlerMethodName, handlerType?.Name, _assembly.GetName().Name);
        }
        
        var result = (Task<IActionResult>) handler?.Invoke(handlerInstance, new []{update});
        return result!;
    }
}