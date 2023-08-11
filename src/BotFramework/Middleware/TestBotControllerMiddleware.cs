// using System;
// using System.Text;
// using System.Threading.Tasks;
// using BotFramework.Db.Entity;
// using BotFramework.Exceptions;
// using BotFramework.Extensions;
// using BotFramework.Repository;
// using MapsterMapper;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Telegram.Bot;
// using Telegram.Bot.Types;
//
// namespace BotFramework.Middleware;
//
// public class TestBotControllerMiddleware
// {
//     private readonly RequestDelegate _next;
//     private IBaseBotRepository _botRepository;
//     private IMapper _mapper;
//
//     public TestBotControllerMiddleware(RequestDelegate next)
//     {
//         _next = next;
//     }
//
//     public async Task InvokeAsync(HttpContext context, IBaseBotRepository botRepository, IMapper mapper, ITelegramBotClient bot)
//     {
//         
//         await _next.Invoke(context);
//
//     }
//
//     private bool IsBotRequest(HttpContext context)
//     {
//         HttpRequest request = context.Request;
//         return string.IsNullOrEmpty(context.Request.Path) && 
//                request.Method == HttpMethods.Post && 
//                request.ContentLength > 0;
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
