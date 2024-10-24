// using System.Reflection;
// using Microsoft.AspNetCore.Mvc;
// using MultipleBotFramework.Dispatcher;
// using MultipleBotFramework.Repository;
// using Swashbuckle.AspNetCore.Annotations;
// using Telegram.BotAPI.GettingUpdates;
//
// namespace MultipleTestBot.Controller;
//
// [ApiController]
// [SwaggerTag("Сервисные эндпоинты для ботов.")]
// public class MainBotDispatcherController : BotDispatcherController
// {
//     private IBaseBotRepository _baseBotRepository;
//
//     public MainBotDispatcherController(IBaseBotRepository baseBotRepository,
//         IHttpContextAccessor contextAccessor) 
//         : base(baseBotRepository, contextAccessor, Assembly.GetExecutingAssembly())
//     {   
//     }
//     
//     /// <summary>
//     /// Эндпонит для привязки webhook к телеграм обновлениям.
//     /// </summary>
//     /// <param name="botId"></param>
//     /// <param name="updateRequest"></param>
//     /// <returns></returns>
//     [HttpPost("/{botId}")]
//     public async Task<IActionResult> Handle(long botId, [FromBody] Update updateRequest)
//     {
//         // return await base.HandleBotRequest(botId, updateRequest);
//         return await HandleBotRequest(botId, updateRequest);
//     }
//     
//     /// <summary>
//     /// Не использовать этот эндпоинт нигде.
//     /// </summary>
//     /// <param name="botId"></param>
//     /// <param name="update"></param>
//     /// <returns></returns>
//     [HttpPost("/handle-bot-update")]
//     public override Task<IActionResult> HandleBotRequest(long botId, Update update)
//     {
//         return base.HandleBotRequest(botId, update);
//     }
// }