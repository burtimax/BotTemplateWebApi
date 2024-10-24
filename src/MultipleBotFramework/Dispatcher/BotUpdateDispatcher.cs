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
using MultipleBotFramework.Constants;
using MultipleBotFramework.Db;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Dispatcher.HandlerResolvers;
using MultipleBotFramework.Dto;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Exceptions;
using MultipleBotFramework.Extensions;
using MultipleBotFramework.Options;
using MultipleBotFramework.Repository;
using MultipleBotFramework.Services;
using MultipleBotFramework.Services.Interfaces;
using MultipleBotFramework.Utils;
using MultipleBotFramework.Utils.ExceptionHandler;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.GettingUpdates;

namespace MultipleBotFramework.Dispatcher;

[Controller]
public class BotUpdateDispatcher
{
    private static int t = 0;

    private readonly ILogger _logger;
    private readonly IBaseBotRepository _botRepository;
    private readonly Assembly[] _assemblies;
    private readonly BotOptions _botOptions;
    private readonly SaveUpdateService _saveUpdateService;
    private readonly BotConfiguration _botConfiguration;
    private readonly BotDbContext _db;
    private readonly ISavedMessageService _savedMessageService;
    private readonly IServiceProvider _serviceProvider;
    private readonly BotChatHistoryService _chatHistoryService;

    public BotUpdateDispatcher(
        IServiceProvider serviceProvider)
    {
        _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        _serviceProvider = serviceProvider;
        _botRepository = serviceProvider.GetRequiredService<IBaseBotRepository>();
        _saveUpdateService = _serviceProvider.GetRequiredService<SaveUpdateService>();
        _botConfiguration = _serviceProvider.GetRequiredService<IOptions<BotConfiguration>>().Value;
        _db = _serviceProvider.GetRequiredService<BotDbContext>();
        _chatHistoryService = serviceProvider.GetRequiredService<BotChatHistoryService>();
        var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();
        _botOptions = (_serviceProvider.GetRequiredService<IOptions<BotOptions>>())?.Value ?? new();
        _savedMessageService = _serviceProvider.GetRequiredService<ISavedMessageService>();
        _logger = loggerFactory.CreateLogger("Bot");
    }

    public async Task HandleBotRequest(long botId, Update update)
    {
        // Объявим здесь. Инициализируем далее.
        BotUserEntity? user = null;
        BotChatEntity? chat = null;
        IEnumerable<ClaimValue>? userClaims;
        BotUpdateEntity savedUpdateEntity = null;
        
        // Определяем для какого бота пришел запрос.
        IBotFactory botFactory = _serviceProvider.GetRequiredService<IBotFactory>();
        ITelegramBotClient? botClient = await botFactory.GetInstance(botId);
        if (botClient is null) return;
        
        try
        {
            if (update == null) throw new NullUpdateModelInMiddleWareException();
            
            User? telegramUser = update.GetUser();
            Chat? telegramChat = update.GetChat();
            
            // Сохраняем или обновляем информацию о пользователе.
            user = await _botRepository.UpsertUser(botId, telegramUser);
            userClaims = (await _botRepository.GetUserClaims(botId, user?.Id ?? -1))?.Select(c => new ClaimValue(c.Id, c.Name, c?.Description ?? ""));
            bool isOwner = await _botRepository.IsUserOwner(botId, user?.TelegramId ?? -1);

            BotChatEntity? existedChat = null;
            if (telegramChat is not null)
            {
                chat = existedChat ?? await _botRepository.UpsertChat(botId, telegramChat, telegramUser);
                await _chatHistoryService.SaveInChatHistoryIfNeed(botId, chat.TelegramId, false, update);
            }
            
            // Если пользователь заблокирован, тогда ему не отвечаем!!!
            if (user != null && user.IsBlocked)
            {
                // ToDo перенаправить на состояние блокированного пользователя!!!
                if(chat != null)
                    await botClient.SendMessageAsync(chat.ChatId, "Вы были заблокированы модератором");
                return;
            }
            
            // Сохраняем запрос в истории бота.
            if (_botOptions.SaveUpdatesInDatabase)
            {
                savedUpdateEntity = await _saveUpdateService.SaveUpdateInBotHistory(botId, user, chat, update);
            }

            // Если пришло сообщение из медиа группы сохраненного сообщения, то просто сохранить и ничего не делать.
            // Когда мы сохраняем в бота сообщение с несколькими медиа, то запросы по каждому медиа приходят поотдельности.
            // Нужно собрать все вместе. Вставляем в Диспетчер метод сохранения и сохраняем другие медиа рядом.
            if (update.Type() == UpdateType.Message &&
                string.IsNullOrEmpty(update.Message.MediaGroupId) == false &&
                await _savedMessageService.HasSavedMessageWithMediaType(botId, chat?.TelegramId, user?.TelegramId, update.Message.MediaGroupId))
            {
                await _savedMessageService.SaveMessageFromUpdate(botId, chat, user, update.Message);
                return;
            }
            
            // Отключенным чатам не отвечаем.
            if (chat.DisabledUntil is not null && DateTime.Now <= chat.DisabledUntil) return;
            
            // Получаем текушее состояние чата. 
            string currentState = chat?.States?.CurrentState ?? BotConstants.StartState;
            BotHandlerResolver handlerResolver = new(_assemblies);
            BotHandlerResolverArgs resolverArgs = new()
            {
                BotId = botId,
                UserRole = user?.Role,
                User = user,
                UserClaims = userClaims,
                UpdateType = update.Type(),
                StateName = currentState,
                Command = update.Type() == UpdateType.Command ? update!.Message!.Text : null,
                ChatType = chat?.GetType() ?? ChatType.Unknown,
            };

            List<Type>? handlerTypes = handlerResolver.GetHandlers(resolverArgs);
            if (handlerTypes is null || handlerTypes.Any() == false)
            {
                throw new Exception("Не нашел обработчика для состояния");
            }

            var handlerType = handlerTypes.First();
            await ProcessRequestByHandler<IBaseBotHandler>(handlerType, botId, isOwner, botClient, update, chat, user, savedUpdateEntity, userClaims);
            return;
        }
        catch (Exception e)
        {
            _logger.LogError(LogFormat.ExceptionUpdate, savedUpdateEntity?.Id.ToString() ?? "null", e.Message);
            
            BotExceptionHandler exceptionHandler = new();
            BotExceptionHandlerArgs args = new(e, _serviceProvider)
            {
                BotId = botId,
                TelegramUpdate = update,
                BotUpdate = savedUpdateEntity,
                BotUser = user,
                BotChat = chat,
            };
            await exceptionHandler.Handle(args);
            
            return;
        }

        await SendUnprocessedUpdateToModerators(botClient, botId, update);
        return;

        // Отправляем ответ пользователю
    }

    /// <summary>
    /// Отправить необработанный запрос модератору.
    /// </summary>
    private async Task SendUnprocessedUpdateToModerators(ITelegramBotClient botClient, long botId, Update update)
    {
        await BotHelper.ExecuteFor(_db, botId, BotConstants.BaseBotClaims.BotExceptionsGet, async (tuple) =>
        {
            await botClient.SendMessageAsync(tuple.chat.ChatId,
                $"Бот не может найти обработчик для запроса\n<code>{update.ToJson()}</code>", parseMode:ParseMode.Html);
        });
    }

    /// <summary>
    /// Запустить обработчик типа состояния или команды бота и получить результат. 
    /// </summary>
    /// <param name="handlerTypeName">Наименование типа обработчика.</param>
    /// <param name="update">Объект запроса от Telegram API по webhook.</param>
    /// <remarks>T - <see cref="BaseBotHandler"/> или <see cref="BaseBotCommand"/>.</remarks>
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
            update.Type().ToString()
        );
        
        // Assembly handlerTypeAssembly = handlerType.Assembly;
        // T handlerInstance = (T) handlerTypeAssembly.CreateInstance(handlerType.FullName, true, BindingFlags.Default, null,
        //     new object[] { HttpContext.RequestServices }, null, null);

        T handlerInstance = (T) ActivatorUtilities.CreateInstance(_serviceProvider, handlerType, new object[] { _serviceProvider });
        
        if (handlerInstance == null)
        {
            throw new CannotCreateUpdateHandlerInstanceException(handlerType?.Name, handlerType?.Assembly?.GetName()?.Name);
        }
        
        // Инициализируем свойства класса базового состояния бота. 
        handlerInstance.Chat = chat;
        handlerInstance.User = user;
        handlerInstance.Update = update;
        handlerInstance.BotId = botId;
        handlerInstance.BotClient = botClient;
        handlerInstance.IsOwner = isOwner;
        handlerInstance.ServiceProvider = _serviceProvider;
        handlerInstance.UserClaims = userClaims?.ToList()?.AsReadOnly() 
                                     ?? new List<ClaimValue>().AsReadOnly();
        
        // Каждое состояние должно быть наследником типа BaseBotState и реализовывать метод HandleBotRequest

        string handlerMethodName = nameof(IBaseBotHandler.HandleBotRequest);
        MethodInfo? handler = handlerType?.GetMethod(handlerMethodName);

        if (handler == null)
        {
            throw new NotFoundHandlerMethodException(handlerMethodName, handlerType?.Name, handlerType?.Assembly?.GetName()?.Name);
        }
        
        Task result = (Task)handler?.Invoke(handlerInstance, new[] { update });
        
        _logger.LogInformation(LogFormat.ProcessedUpdate, 
            savedUpdate != null ? savedUpdate.Id.ToString() : "NULL");

        return result;
    }
}