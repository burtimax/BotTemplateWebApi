// using System.Security.Claims;
// using BotFramework.TEST;
// using BotTemplateWebApi.Interfaces.IServices;
// using Microsoft.AspNetCore.Mvc;
// using Telegram.Bot;
// using Telegram.Bot.Types;
//
// namespace BotTemplateWebApi.Controllers;
//
// [ApiController] 
// //[Route("")]
// public class TestController : ControllerBase
// {
//     private readonly IBotSingleton _botSingleton;
//     private readonly TelegramBotClient _botClient;
//
//     public TestController(IBotSingleton botSingleton)
//     {
//         _botSingleton = botSingleton;
//         _botClient = botSingleton.GetInstance().ApiClient;
//     }
//     
//     
//     // [HttpPost("/")]
//     // public async Task<IActionResult> Test(Update update)
//     // {
//     //     await _botClient.SendTextMessageAsync(update.Message.Chat.Id, "Привет");
//     //     return Ok();
//     // }
//     
//     [HttpGet("/")]
//     protected async Task<ActionResult> HandleBotRequest(Update update)
//     {
//         return Ok("Привет");
//     }
// }