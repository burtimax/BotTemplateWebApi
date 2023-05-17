// using System.Text;
// using BotFramework.Db.Entity;
// using BotFramework.Extensions;
// using BotFramework.Interfaces;
// using BotTemplateWebApi.App;
// using MapsterMapper;
// using Telegram.Bot.Types;
//
// namespace BotTemplateWebApi.Middleware;
//
// public class BotMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly IBaseBotRepository _botRepository;
//     private readonly IMapper _mapper;
//
//     public BotMiddleware(RequestDelegate next, 
//         IBaseBotRepository botRepository, IMapper mapper)
//     {
//         _next = next;
//         _botRepository = botRepository;
//         _mapper = mapper;
//     }
//
//     public async Task InvokeAsync(HttpContext context)
//     {
//         HttpRequest request = context.Request;
//         Update? update = null;
//         
//         // Если запрос не на бота, тогда идем дальше.
//         if (IsBotRequest(context) == false)
//         {
//             await _next.Invoke(context);
//             return;
//         }
//
//         update = await GetUpdateFromRequest(request);
//
//         if (update == null) throw new Exception(AppExceptions.NullUpdateModelInMiddleWare);
//
//         User telegramUser = update.GetUser();
//         Chat telegramChat = update.GetChat();
//         
//         // Сохраняем или обновляем информацию о пользователе.
//         BotUser user = await _botRepository.GetUser(telegramUser.Id);
//         if (user == null)
//         {
//             await _botRepository.UpsertUser(telegramUser);
//         }
//         else
//         {
//             _botRepository.
//         }
//         
//         // Сохраняем чат, если еще не существует. 
//         
//         // Получаем текушее состояние чата. 
//         
//         // Собираем всю необходимую информацию для дальнейшей обработки состояний и регистрируем в сервисы.
//         
//         // Перенаправляем запрос на сервисы
//         
//         await _next.Invoke(context);
//         
//         // Отправляем ответ пользователю
//     }
//
//     private bool IsBotRequest(HttpContext context)
//     {
//         HttpRequest request = context.Request;
//         return request.Method == HttpMethods.Post && request.ContentLength > 0;
//     }
//     
//     private async Task<Update?> GetUpdateFromRequest(HttpRequest request)
//     {
//         request.EnableBuffering();
//         var buffer = new byte[Convert.ToInt32(request.ContentLength)];
//         await request.Body.ReadAsync(buffer, 0, buffer.Length);
//         //get body string here...
//         var requestContent = Encoding.UTF8.GetString(buffer);
//         request.Body.Position = 0;  //rewinding the stream to 0
//         Update? update = Newtonsoft.Json.JsonConvert.DeserializeObject<Update>(requestContent);
//         return update;
//     }
// }