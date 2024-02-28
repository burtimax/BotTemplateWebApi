using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Base;
using BotFramework.Db;
using BotFramework.Db.Entity;
using BotFramework.Dispatcher.HandlerResolvers;
using BotFramework.Dto;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using BotFramework.Options;
using BotFramework.Other;
using BotFramework.Repository;
using BotFramework.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Dispatcher;

[Controller]
public class BotDispatcherController : BaseBotController
{
    private static int t = 0;

    private readonly ILogger _logger;
    private readonly IBaseBotRepository _botRepository;
    private readonly HttpContext _context;
    private readonly Assembly _assembly;
    private readonly BotOptions _botOptions;
    private readonly ISaveUpdateService _saveUpdateService;
    private readonly BotConfiguration _botConfiguration;
    private readonly ITelegramBotClient _botClient;
    private readonly BotDbContext _db;

    public BotDispatcherController(
        IBaseBotRepository botRepository,
        IHttpContextAccessor contextAccessor,
        Assembly assembly)
    {
        _assembly = assembly;
        _botRepository = botRepository;
        _context = contextAccessor.HttpContext;
        _saveUpdateService = _context.RequestServices.GetRequiredService<ISaveUpdateService>();
        _botConfiguration = _context.RequestServices.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _db = _context.RequestServices.GetRequiredService<BotDbContext>();
        var loggerFactory = _context.RequestServices.GetRequiredService<ILoggerFactory>();
        _botClient = _context.RequestServices.GetRequiredService<ITelegramBotClient>();
        _botOptions = (_context.RequestServices.GetRequiredService<IOptions<BotOptions>>())?.Value ?? new();
        _logger = loggerFactory.CreateLogger("Bot");
    }

    public override async Task<IActionResult> HandleBotRequest(Update update)
    {
        // Объявим здесь. Инициализируем далее.
        BotUser? user = null;
        BotChat? chat = null;
        IEnumerable<ClaimValue>? userClaims;
        BotUpdate savedUpdate = null;
        
        try
        {
            HttpRequest request = _context.Request;

            if (update == null) throw new NullUpdateModelInMiddleWareException();
            
            User? telegramUser = update.GetUser();
            Chat? telegramChat = update.GetChat();

            // Сохраняем или обновляем информацию о пользователе.
            user = await _botRepository.UpsertUser(telegramUser);
            userClaims = (await _botRepository.GetUserClaims(user?.Id ?? -1))?.Select(c => new ClaimValue(c.Id, c.Name, c?.Description ?? ""));
            
            // Сохраняем чат, если еще не существует. 
            BotChat? existedChat = await _botRepository.GetChat(new ChatId(telegramChat?.Id ?? -1), user?.Id ?? -1);
            chat = existedChat ?? await _botRepository.AddChat(telegramChat, user);
            
            // Если пользователь заблокирован, тогда ему не отвечаем!!!
            if (user != null && user.IsBlocked)
            {
                // ToDo перенаправить на состояние блокированного пользователя!!!
                if(chat != null)
                    await _botClient.SendTextMessageAsync(chat.ChatId, "Вы были заблокированы модератором");
                return Ok();
            }
            
            // Сохраняем запрос в истории бота.
            if (_botOptions.SaveUpdatesInDatabase)
            {
                savedUpdate = await _saveUpdateService.SaveUpdateInBotHistory(user, chat, update);
            }

            // Команды бота обрабатываются вне очереди, вне состояний.
            if (user != null && chat != null && IsCommand(update))
            {
                // Ищем обработчик команды.
                string command = update!.Message!.Text!;
                BotCommandHandlerResolver commandHandlerResolver = new(_assembly, Assembly.GetExecutingAssembly());
                Type? commandHandler = commandHandlerResolver.GetPriorityCommandHandlerType(command, user, userClaims);

                if (commandHandler == null)
                {
                    await _botClient.SendTextMessageAsync(chat.ChatId, "Не понимаю 🤷‍♂️");
                    return Ok(); // Сделать так чтобы логировалось и не было ошибок.
                    throw new NotFoundHandlerForCommandException(command, _assembly.GetName().Name);
                }
                
                // Обрабатываем команду.
                await ProcessRequestByHandler<BaseBotCommand>(commandHandler, update, chat, user, savedUpdate, userClaims);
                return Ok();
            }
            
            // Если есть глобальные обработчики запроса, тогда перенаправляем на них.
            BotPriorityHandlerResolver priorityHandler = new(_assembly, Assembly.GetExecutingAssembly());
            Type? updateHandler = priorityHandler.GetPriorityTypeHandler(update.Type);
            if (updateHandler != null)
            {
                await ProcessRequestByHandler<BaseBotPriorityHandler>(updateHandler, update, chat, user, savedUpdate, userClaims);
                return Ok();
            }

            if (user != null && chat != null)
            {
                // Получаем текушее состояние чата. 
                string currentState = chat.States?.CurrentState ?? BotConstants.StartState;
            
                BotStateHandlerResolver resolver = new(_assembly);
                Type handlerType = resolver.GetPriorityStateHandlerType(currentState, user.Role)
                                   ?? throw new NotFoundHandlerForStateException(currentState, _assembly.GetName().Name);
            
                await ProcessRequestByHandler<BaseBotState>(handlerType, update, chat, user, savedUpdate, userClaims);

                return Ok();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(LogFormat.ExceptionUpdate, savedUpdate?.Id.ToString() ?? "null", e.Message);
            
            BotExceptionHandler exceptionHandler = new();
            await exceptionHandler.Handle(e, update, savedUpdate, user, chat, HttpContext.RequestServices);
            
            return Ok();
        }

        await SendUnprocessedUpdateToModerators(update);
        return Ok();

        // Отправляем ответ пользователю
    }

    /// <summary>
    /// Отправить необработанный запрос модератору.
    /// </summary>
    private async Task SendUnprocessedUpdateToModerators(Update update)
    {
        await BotHelper.ExecuteFor(_db, BotConstants.BaseBotClaims.BotExceptionsGet, async (tuple) =>
        {
            await _botClient.SendTextMessageAsync(tuple.chat.ChatId,
                $"Бот не может найти обработчик для запроса\n<code>{JsonConvert.SerializeObject(update, Formatting.Indented)}</code>", parseMode:ParseMode.Html);
        });
    }
    
    /// <summary>
    /// Обрабатываем специфичные запросы.
    /// </summary>
    /// <param name="update"></param>
    /// <returns></returns>
    private async Task<bool> HandleSpecifiedRequests(Update update)
    {
        // Необходимо обработать Poll тип запроса, потому что у него нет данных о пользователе и чате.
        if (update.Type == UpdateType.Poll)
        {
            //await (_botOptions?.PollHandler?.Invoke(update.Poll) ?? DefaultHandlers.DefaultPoll.Handler(update.Poll));
            return true;
        }

        return false;
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
    private bool IsCommand(Update update) =>
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
    private Task ProcessRequestByHandler<T>(Type handlerType, Update update, BotChat? chat, BotUser? user, BotUpdate? savedUpdate, IEnumerable<ClaimValue>? userClaims) where T : IBaseBotHandler
    {
        if (handlerType == null) throw new ArgumentNullException(nameof(handlerType));

        _logger.LogInformation(LogFormat.ReceiveUpdate, 
            savedUpdate != null ? savedUpdate.Id.ToString() : "NULL", 
            $"{user?.TelegramId.ToString() ?? "UnknownUser"}/@{user?.TelegramUsername ?? "_"}",
            $"{chat?.TelegramId.ToString() ?? "_"}",
            $"{chat?.States?.CurrentState ?? "_"}",
            update.Type.ToString()
        );
        
        Assembly handlerTypeAssembly = handlerType.Assembly;
        
        T handlerInstance = (T) handlerTypeAssembly.CreateInstance(handlerType.FullName, true, BindingFlags.Default, null,
            new object[] { HttpContext.RequestServices }, null, null);

        if (handlerInstance == null)
        {
            throw new CannotCreateUpdateHandlerInstanceException(handlerType?.Name, _assembly.GetName().Name);
        }
        
        // Инициализируем свойства класса базового состояния бота. 
        handlerInstance.Chat = chat;
        handlerInstance.User = user;
        handlerInstance.Update = update;
        handlerInstance.UserClaims = userClaims?.ToList()?.AsReadOnly() 
                                     ?? new List<ClaimValue>().AsReadOnly();
        
        // Каждое состояние должно быть наследником типа BaseBotState и реализовывать метод HandleBotRequest

        string handlerMethodName = nameof(IBaseBotHandler.HandleBotRequest);
        MethodInfo? handler = handlerType?.GetMethod(handlerMethodName);

        if (handler == null)
        {
            throw new NotFoundHandlerMethodException(handlerMethodName, handlerType?.Name, _assembly.GetName().Name);
        }
        
        Task result = (Task)handler?.Invoke(handlerInstance, new[] { update });
        
        _logger.LogInformation(LogFormat.ProcessedUpdate, 
            savedUpdate != null ? savedUpdate.Id.ToString() : "NULL");

        return result;
    }
}