using System.Reflection;
using System.Security.Claims;
using BotFramework.Controllers;
using BotFramework.Repository;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotTemplateWebApi.Controllers;

[ApiController]
public class MainBotDispatcherController : BotDispatcherController
{
    private readonly ITelegramBotClient _botClient;
    private readonly IMapper _mapper;
    private IBaseBotRepository _baseBotRepository;

    public MainBotDispatcherController(ITelegramBotClient botClient, IMapper mapper, IBaseBotRepository baseBotRepository,
        IHttpContextAccessor contextAccessor) 
        : base(mapper, baseBotRepository, contextAccessor, Assembly.GetExecutingAssembly())
    {
        _botClient = botClient;
    }

    [HttpPost("/")]
    public override async Task<IActionResult> HandleBotRequest([FromBody] Update updateRequest)
    {
        return await base.HandleBotRequest(updateRequest);
    }
}