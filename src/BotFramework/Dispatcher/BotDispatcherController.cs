using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using BotFramework.Attributes;
using BotFramework.Base;
using BotFramework.Db.Entity;
using BotFramework.Dispatcher;
using BotFramework.Dispatcher.HandlerResolvers;
using BotFramework.Dto;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using BotFramework.Options;
using BotFramework.Other;
using BotFramework.Repository;
using BotFramework.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;

namespace BotFramework.Controllers;

[Controller]
public class BotDispatcherController : BaseBotController
{
    private static int t = 0;

    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IBaseBotRepository _botRepository;
    private readonly HttpContext _context;
    private readonly Assembly _assembly;
    private readonly BotOptions _botOptions;
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
        var loggerFactory = _context.RequestServices.GetRequiredService<ILoggerFactory>();
        _botOptions = (_context.RequestServices.GetRequiredService<IOptions<BotOptions>>())?.Value ?? new();
        _logger = loggerFactory.CreateLogger("Bot");
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        // Объявим здесь. Инициализируем далее.
        BotUser user = null;
        BotChat chat = null;
        BotUpdate savedUpdate = null;
        
        try
        {
            HttpRequest request = _context.Request;

            if (update == null) throw new NullUpdateModelInMiddleWareException();

            User? telegramUser = update.GetUser();
            Chat? telegramChat = update.GetChat();

            // Сохраняем или обновляем информацию о пользователе.
            user = await _botRepository.UpsertUser(telegramUser);

            if (user == null)
            {
                // ToDo добавить отдельный обработчик для типов обновлений с нулевым пользователем.
                throw new NotImplementedException("Сейчас пока типы обновлений, у которых User == null не обрабатываются");
            }
            
            // Сохраняем чат, если еще не существует. 
            BotChat? existedChat = await _botRepository.GetChat(telegramChat?.Id ?? user.Id);
            chat = existedChat ?? await _botRepository.AddChat(telegramChat, user);
            
            // Сохраняем запрос в истории бота.
            if (_botOptions.SaveUpdatesInDatabase)
            {
                savedUpdate = await _saveUpdateService.SaveUpdateInBotHistory(user, chat, update);
            }

            // Команды бота обрабатываются вне очереди, вне состояний.
            if (IsBotCommandUpdate(update))
            {
                // Ищем обработчик команды.
                string command = update.Message.Text;
                BotCommandHandlerResolver commandHandlerResolver = new(_assembly);
                Type commandHandler = commandHandlerResolver.GetPriorityCommandHandlerType(command, user.Role)
                    ?? throw new NotFoundHandlerForCommandException(command, _assembly.GetName().Name);
                
                // Обрабатываем команду.
                await ProcessRequestByHandler<BaseBotCommand>(commandHandler, update, chat, user);
                return Ok();
            }
            
            // Получаем текушее состояние чата. 
            string currentState = chat.States.CurrentState;

            _logger.LogInformation(LogFormat.ReceiveUpdate, 
                savedUpdate.Id.ToString(), 
                $"{user?.TelegramId.ToString() ?? "UnknownUser"}/@{user?.TelegramUsername ?? "_"}",
                $"{chat?.TelegramId.ToString() ?? "_"}",
                $"{chat?.States?.CurrentState ?? "_"}",
                update.Type.ToString()
            );

            BotStateHandlerResolver resolver = new(_assembly);
            Type handlerType = resolver.GetPriorityStateHandlerType("GetAudioMessageSample", user.Role)
                ?? throw new NotFoundHandlerForStateException(currentState, _assembly.GetName().Name);
            
            await ProcessRequestByHandler<BaseBotState>(handlerType, update, chat, user);
            
            _logger.LogInformation(LogFormat.ProcessedUpdate, savedUpdate.Id.ToString());

            return Ok();
        }
        catch (Exception e)
        {
            _logger.LogError(LogFormat.ExceptionUpdate, savedUpdate.Id, e.Message);
            
            BotExceptionHandler exceptionHandler = new();
            await exceptionHandler.Handle(e, update, user, chat, HttpContext.RequestServices);

            return Ok();
        }

        return Ok();

        // Отправляем ответ пользователю
    }

    /// <summary>
    /// Является ли запрос командой бота.
    /// </summary>
    /// <remarks>
    /// Команды представлены в виде текста, начинающегося с '/'.
    /// Например "/command".
    /// </remarks>
    /// <param name="update">Запрос бота.</param>
    /// <returns>Является или не является командой.</returns>
    private bool IsBotCommandUpdate(Update update) =>
        update.Type == UpdateType.Message && 
        update.Message.Type == MessageType.Text &&
        update.Message.Text.StartsWith("/");

    /// <summary>
    /// Запустить обработчик типа состояния или команды бота и получить результат. 
    /// </summary>
    /// <param name="handlerTypeName">Наименование типа обработчика.</param>
    /// <param name="update">Объект запроса от Telegram API по webhook.</param>
    /// <remarks>T - <see cref="BaseBotState"/> или <see cref="BaseBotCommand"/>.</remarks>
    /// <returns></returns>
    /// <exception cref="NotFoundHandlerForStateException">Не найден тип обработчика запроса.</exception>
    /// <exception cref="NotFoundHandlerMethodException">Не найден метод обработчика запроса.</exception>
    private Task ProcessRequestByHandler<T>(Type handlerType, Update update, BotChat chat, BotUser user) where T : IBaseBotHandler
    {
        if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));
        
        T handlerInstance = (T) _assembly.CreateInstance(handlerType.FullName, true, BindingFlags.Default, null,
            new object[] { HttpContext.RequestServices }, null, null);

        if (handlerInstance == null)
        {
            throw new CannotCreateUpdateHandlerInstanceException(handlerType?.Name, _assembly.GetName().Name);
        }
        
        // Инициализируем свойства класса базового состояния бота. 
        handlerInstance.Chat = chat;
        handlerInstance.User = user;
        
        // Каждое состояние должно быть наследником типа BaseBotState и реализовывать метод HandleBotRequest

        string handlerMethodName = nameof(IBaseBotHandler.HandleBotRequest);
        MethodInfo? handler = handlerType?.GetMethod(handlerMethodName);

        if (handler == null)
        {
            throw new NotFoundHandlerMethodException(handlerMethodName, handlerType?.Name, _assembly.GetName().Name);
        }
        
        return (Task)handler?.Invoke(handlerInstance, new[] { update });
    }
}