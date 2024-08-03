using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultipleBotFramework.Base;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Exceptions;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services;
using MultipleBotFramework.Utils;
using MultipleBotFramework.Utils.ExceptionHandler;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MultipleBotFramework.Dispatcher;

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
    private readonly BotDbContext _db;
    private readonly ISavedMessageService _savedMessageService;

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
        _botOptions = (_context.RequestServices.GetRequiredService<IOptions<BotOptions>>())?.Value ?? new();
        _savedMessageService = _context.RequestServices.GetRequiredService<ISavedMessageService>();
        _logger = loggerFactory.CreateLogger("Bot");
    }

    public override async Task<IActionResult> HandleBotRequest(long botId, Update update)
    {
        // Объявим здесь. Инициализируем далее.
        BotUserEntity? user = null;
        BotChatEntity? chat = null;
        IEnumerable<ClaimValue>? userClaims;
        BotUpdateEntity savedUpdateEntity = null;
        
        // Определяем для какого бота пришел запрос.
        IBotFactory botFactory = _context.RequestServices.GetRequiredService<IBotFactory>();
        ITelegramBotClient? botClient = botFactory.GetInstance(botId);
        if (botClient is null) return Ok();
        
        try
        {
            HttpRequest request = _context.Request;

            if (update == null) throw new NullUpdateModelInMiddleWareException();
            
            User? telegramUser = update.GetUser();
            Chat? telegramChat = update.GetChat();

            // Сохраняем или обновляем информацию о пользователе.
            user = await _botRepository.UpsertUser(botId, telegramUser);
            userClaims = (await _botRepository.GetUserClaims(botId, user?.Id ?? -1))?.Select(c => new ClaimValue(c.Id, c.Name, c?.Description ?? ""));
            bool isOwner = await _botRepository.IsUserOwner(botId, user.TelegramId);
            
            // Сохраняем чат, если еще не существует. 
            BotChatEntity? existedChat = await _botRepository.GetChat(botId, new ChatId(telegramChat?.Id ?? -1), user?.Id ?? -1);
            chat = existedChat ?? await _botRepository.AddChat(botId, telegramChat, user);
            
            // Если пользователь заблокирован, тогда ему не отвечаем!!!
            if (user != null && user.IsBlocked)
            {
                // ToDo перенаправить на состояние блокированного пользователя!!!
                if(chat != null)
                    await botClient.SendTextMessageAsync(chat.ChatId, "Вы были заблокированы модератором");
                return Ok();
            }
            
            // Сохраняем запрос в истории бота.
            if (_botOptions.SaveUpdatesInDatabase)
            {
                savedUpdateEntity = await _saveUpdateService.SaveUpdateInBotHistory(botId, user, chat, update);
            }

            // Если пришло сообщение из медиа группы сохраненного сообщения, то просто сохранить и ничего не делать.
            // Когда мы сохраняем в бота сообщение с несколькими медиа, то запросы по каждому медиа приходят поотдельности.
            // Нужно собрать все вместе. Вставляем в Диспетчер метод сохранения и сохраняем другие медиа рядом.
            if (update.Type == UpdateType.Message &&
                string.IsNullOrEmpty(update.Message.MediaGroupId) == false &&
                await _savedMessageService.HasSavedMessageWithMediaType(botId, chat.TelegramId.Value, user.TelegramId, update.Message.MediaGroupId))
            {
                await _savedMessageService.SaveMessageFromUpdate(botId, chat, user, update.Message);
                return Ok();
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
                    await botClient.SendTextMessageAsync(chat.ChatId, "Не понимаю 🤷‍♂️");
                    return Ok(); // Сделать так чтобы логировалось и не было ошибок.
                    throw new NotFoundHandlerForCommandException(command, _assembly.GetName().Name);
                }
                
                // Обрабатываем команду.
                await ProcessRequestByHandler<BaseBotCommand>(commandHandler, botId, isOwner, botClient, update, chat, user, savedUpdateEntity, userClaims);
                return Ok();
            }
            
            // Если есть глобальные обработчики запроса, тогда перенаправляем на них.
            BotPriorityHandlerResolver priorityHandler = new(_assembly, Assembly.GetExecutingAssembly());
            Type? updateHandler = priorityHandler.GetPriorityTypeHandler(update.Type);
            if (updateHandler != null)
            {
                await ProcessRequestByHandler<BaseBotPriorityHandler>(updateHandler, botId, isOwner, botClient, update, chat, user, savedUpdateEntity, userClaims);
                return Ok();
            }

            if (user != null && chat != null)
            {
                // Получаем текушее состояние чата. 
                string currentState = chat.States?.CurrentState ?? BotConstants.StartState;
            
                BotStateHandlerResolver resolver = new(_assembly, Assembly.GetExecutingAssembly());
                Type handlerType = resolver.GetPriorityStateHandlerType(currentState, user.Role)
                                   ?? throw new NotFoundHandlerForStateException(currentState, _assembly.GetName().Name);
            
                await ProcessRequestByHandler<BaseBotState>(handlerType, botId, isOwner, botClient, update, chat, user, savedUpdateEntity, userClaims);

                return Ok();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(LogFormat.ExceptionUpdate, savedUpdateEntity?.Id.ToString() ?? "null", e.Message);
            
            BotExceptionHandler exceptionHandler = new();
            BotExceptionHandlerArgs args = new(e, HttpContext.RequestServices)
            {
                BotId = botId,
                TelegramUpdate = update,
                BotUpdate = savedUpdateEntity,
                BotUser = user,
                BotChat = chat,
            };
            await exceptionHandler.Handle(args);
            
            return Ok();
        }

        await SendUnprocessedUpdateToModerators(botClient, botId, update);
        return Ok();

        // Отправляем ответ пользователю
    }

    /// <summary>
    /// Отправить необработанный запрос модератору.
    /// </summary>
    private async Task SendUnprocessedUpdateToModerators(ITelegramBotClient botClient, long botId, Update update)
    {
        await BotHelper.ExecuteFor(_db, botId, BotConstants.BaseBotClaims.BotExceptionsGet, async (tuple) =>
        {
            await botClient.SendTextMessageAsync(tuple.chat.ChatId,
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
    private Task ProcessRequestByHandler<T>(Type handlerType, long botId, bool isOwner, ITelegramBotClient botClient, Update update, BotChatEntity? chat, BotUserEntity? user, BotUpdateEntity? savedUpdate, IEnumerable<ClaimValue>? userClaims) where T : IBaseBotHandler
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
        handlerInstance.BotId = botId;
        handlerInstance.BotClient = botClient;
        handlerInstance.IsOwner = isOwner;
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