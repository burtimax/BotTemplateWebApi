// using Microsoft.AspNetCore.Mvc;
// using MultipleTestBot.Endpoints.Bot.GetBots.RequestResponse;
// using MultipleTestBot.Endpoints.Common;
// using MultipleTestBot.Endpoints.Common.Response;
// using MultipleTestBot.Exceptions;
// using MultipleTestBot.Models;
// using MultipleTestBot.Repository.Bot;
// using MultipleTestBot.Repository.Bot.Dto;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace MultipleTestBot.Endpoints.Bot.GetBots;
//
// [ApiController]
// [Route("/api")]
// public class GetBotsEndpoint : BaseEndpoint<GetBotsRequest, PagedList<BotResponse>>
// {
//     private readonly IBotRepository _botRepository;
//     
//     public GetBotsEndpoint(IServiceProvider serviceProvider, 
//         IBotRepository botRepository) : base(nameof(GetBotsRequest), serviceProvider)
//     {
//         _botRepository = botRepository;
//     }
//     
//     [HttpGet("/bot")]
//     [SwaggerOperation(
//         Summary = "Получить список ботов.",
//         Description = "Возвращает список ботов.",
//         OperationId = "Bot.Get",
//         Tags = new[] { "Bot" })]
//     public override Task<ActionResult<BaseResponse<PagedList<BotResponse>>>> HandleAsync([FromQuery] GetBotsRequest request, CancellationToken cancellationToken = new CancellationToken())
//     {
//         return base.HandleAsync(request, cancellationToken);
//     }
//     
//     protected override async Task<PagedList<BotResponse>> HandleEndpoint(GetBotsRequest request, CancellationToken cancellationToken = new CancellationToken())
//     {
//         GetBotsDto dto = _mapper.Map<GetBotsDto>(request);
//         var bots = await _botRepository.GetBots(dto);
//         return _mapper.Map<PagedList<BotResponse>>(bots);
//     }
// }