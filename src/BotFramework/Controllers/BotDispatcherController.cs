using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Attributes;
using BotFramework.Db.Entity;
using BotFramework.Dto;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using BotFramework.Filters;
using BotFramework.Interfaces;
using BotFramework.Other;
using BotFramework.Repository;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotFramework.Controllers;

[Controller]
public class BotDispatcherController : BaseBotController
{
    private static int t = 0;

    private readonly IMapper _mapper;
    private readonly IBaseBotRepository _botRepository;
    private readonly HttpContext _context;
    private readonly Assembly _assembly;

    private readonly ISaveUpdateService _saveUpdateService;

    public BotDispatcherController(IMapper mapper, 
        IBaseBotRepository botRepository,
        IHttpContextAccessor contextAccessor,
        Assembly assembly)
    {
        _assembly = assembly;
        _mapper = mapper;
        _botRepository = botRepository;
        _context = contextAccessor.HttpContext;
        _saveUpdateService = _context.RequestServices.GetRequiredService<ISaveUpdateService>();
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        // Объявим здесь. Инициализируем далее.
        BotUser user = null;
        BotChat chat = null;
        
        try
        {
            HttpRequest request = _context.Request;

            if (update == null) throw new NullUpdateModelInMiddleWareException();

            User telegramUser = update.GetUser();
            Chat telegramChat = update.GetChat();

            // Сохраняем или обновляем информацию о пользователе.
            user = await _botRepository.UpsertUser(telegramUser);

            // Сохраняем чат, если еще не существует. 
            BotChat? existedChat = await _botRepository.GetChat(telegramChat.Id);
            chat = existedChat ?? await _botRepository.AddChat(telegramChat, user);
            
            await _saveUpdateService.SaveUpdateInBotHistory(user, chat, update);

            // Получаем текушее состояние чата. 
            string currentState = chat.States.CurrentState;

            BotHandlerResolver resolver = new(_assembly);
            Type? handlerType = resolver.GetPriorityStateHandlerType("Test", user.Role);

            return await ProcessRequestByHandler(handlerType, update, chat, user);
        }
        catch (Exception e)
        {
            BotExceptionHandler exceptionHandler = new();
            await exceptionHandler.Handle(e, update, user, chat, HttpContext.RequestServices);

            return Ok();
        }

        return Ok();

        // Отправляем ответ пользователю
    }

    /// <summary>
    /// Запустить обработчик типа состояния бота и получить результат. 
    /// </summary>
    /// <param name="handlerTypeName">Наименование типа обработчика.</param>
    /// <param name="update">Объект запроса от Telegram API по webhook.</param>
    /// <returns></returns>
    /// <exception cref="NotFoundHandlerTypeException">Не найден тип обработчика запроса.</exception>
    /// <exception cref="NotFoundHandlerMethodException">Не найден метод обработчика запроса.</exception>
    private Task<IActionResult> ProcessRequestByHandler(Type handlerType, Update update, BotChat chat, BotUser user)
    {
        if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));
        
        BaseBotState handlerInstance = (BaseBotState) _assembly.CreateInstance(handlerType.FullName, true, BindingFlags.Default, null,
            new object[] { HttpContext.RequestServices }, null, null);

        if (handlerInstance == null)
        {
            throw new CannotCreateUpdateHandlerInstanceException(handlerType?.Name, _assembly.GetName().Name);
        }
        
        // Инициализируем свойства класса базового состояния бота. 
        handlerInstance.Chat = chat;
        handlerInstance.User = user;
        
        // Каждое состояние должно быть наследником типа BaseBotState и реализовывать метод HandleBotRequest
        string handlerMethodName = nameof(BaseBotState.HandleBotRequest);

        MethodInfo? handler = handlerType?.GetMethod(handlerMethodName);

        if (handler == null)
        {
            throw new NotFoundHandlerMethodException(handlerMethodName, handlerType?.Name, _assembly.GetName().Name);
        }

        var result = (Task<IActionResult>)handler?.Invoke(handlerInstance, new[] { update });
        return result!;
    }
}