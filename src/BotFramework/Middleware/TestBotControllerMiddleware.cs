using System;
using System.Text;
using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Exceptions;
using BotFramework.Extensions;
using BotFramework.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace BotFramework.Middleware;

public class TestBotControllerMiddleware
{
    private readonly RequestDelegate _next;
    private IBaseBotRepository _botRepository;
    private IMapper _mapper;

    public TestBotControllerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IBaseBotRepository botRepository, IMapper mapper)
    {
        // _mapper = mapper;
        // _botRepository = botRepository;
        //  HttpRequest request = context.Request;
        //  Update? update = null;
        //
        //  // Если запрос не на бота, тогда идем дальше.
        //  if (IsBotRequest(context) == false)
        //  {
        //      await _next.Invoke(context);
        //      return;
        //  }
        //
        //  update = await GetUpdateFromRequest(request);
        //
        //  if (update == null) throw new NullUpdateModelInMiddleWareException();
        //
        //  User telegramUser = update.GetUser();
        //  Chat telegramChat = update.GetChat();
        //
        //  // Сохраняем или обновляем информацию о пользователе.
        //  BotUser user = await _botRepository.UpsertUser(telegramUser);
        //
        //  // Сохраняем чат, если еще не существует. 
        //  BotChat? existedChat = await _botRepository.GetChat(telegramChat.Id);
        //  BotChat chat = existedChat ?? await _botRepository.AddChat(telegramChat, user);
        //
        //  // Получаем текушее состояние чата. 
        // string currentState = chat.States.CurrentState;
        //
        // // Собираем всю необходимую информацию для дальнейшей обработки состояний 
        // // Получили состояние чата, теперь меняем строку запроса и переходим в новый контроллер уже дальше по pipeline.
        //
        // context.Request.Path = "/";

        // Перенаправляем запрос на сервисы
        
        if (context.Response.StatusCode == 302)
        {
            context.Response.StatusCode = 200;
        }
        
        await _next.Invoke(context);
        
        if (context.Response.StatusCode == 302)
        {
            context.Response.StatusCode = 200;
        }
        
        // Отправляем ответ пользователю
    }

    private bool IsBotRequest(HttpContext context)
    {
        HttpRequest request = context.Request;
        return string.IsNullOrEmpty(context.Request.Path) && 
               request.Method == HttpMethods.Post && 
               request.ContentLength > 0;
    }
    
    private async Task<Update?> GetUpdateFromRequest(HttpRequest request)
    {
        request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        //get body string here...
        var requestContent = Encoding.UTF8.GetString(buffer);
        request.Body.Position = 0;  //rewinding the stream to 0
        Update? update = Newtonsoft.Json.JsonConvert.DeserializeObject<Update>(requestContent);
        return update;
    }
}
