using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using BotFramework.Filters;
using BotFramework.Interfaces;
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
            Type baseBotState = typeof(BaseBotState);
            IEnumerable<Type> types = _assembly.GetExportedTypes().Where(t => t.IsAssignableFrom(baseBotState));
            
            return await ProcessRequestByHandler("BotTemplateWebApi.States.TestBot.TestBotState", updateRequest) ;
        }
        catch (Exception e)
        {
            return Ok();
        }
       
       return Ok();

        // Отправляем ответ пользователю
    }


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