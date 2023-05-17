using System.Reflection;
using System.Security.Claims;
using BotFramework.Controllers;
using BotFramework.Filters;
using BotFramework.Interfaces;
using BotTemplateWebApi.Interfaces.IServices;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.Controllers;

[ApiController]
public class MainBotDispatcherController : BotDispatcherController
{
    private readonly IBotSingleton _botSingleton;
    private readonly TelegramBotClient _botClient;
    private readonly IMapper _mapper;
    private IBaseBotRepository _baseBotRepository;

    public MainBotDispatcherController(IBotSingleton botSingleton, IMapper mapper, IBaseBotRepository baseBotRepository,
        IHttpContextAccessor contextAccessor) 
        : base(mapper, baseBotRepository, contextAccessor, Assembly.GetExecutingAssembly())
    {
        _botSingleton = botSingleton;
        var bot = botSingleton.GetInstance();
        _botClient = bot.ApiClient;
    }

    [HttpPost("/")]
    public override async Task<IActionResult> HandleBotRequest([FromBody] Update updateRequest)
    {
        int t = 10;
        await _botClient.SendTextMessageAsync(672312299, "Dispatcher");
        //return Ok();
        return await base.HandleBotRequest(updateRequest);
    }
}