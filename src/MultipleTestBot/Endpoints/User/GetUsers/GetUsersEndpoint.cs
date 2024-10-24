// using Microsoft.AspNetCore.Mvc;
// using MultipleTestBot.Endpoints.Bot.GetBots.RequestResponse;
// using MultipleTestBot.Endpoints.Common;
// using MultipleTestBot.Endpoints.Common.Response;
// using MultipleTestBot.Endpoints.User.GetUsers.RequestResponse;
// using MultipleTestBot.Models;
// using MultipleTestBot.Repository.Bot;
// using MultipleTestBot.Repository.Bot.Dto;
// using MultipleTestBot.Repository.User;
// using MultipleTestBot.Repository.User.Dto;
// using Swashbuckle.AspNetCore.Annotations;
//
// namespace MultipleTestBot.Endpoints.User.GetUsers;
//
// [ApiController]
// [Route("/api")]
// public class GetUsersEndpoint : BaseEndpoint<GetUsersRequest, PagedList<UserResponse>>
// {
//     private readonly IUserRepository _userRepository;
//     
//     public GetUsersEndpoint(IServiceProvider serviceProvider, 
//         IUserRepository userRepository) : base(nameof(GetBotsRequest), serviceProvider)
//     {
//         _userRepository = userRepository;
//     }
//     
//     [HttpGet("/user")]
//     [SwaggerOperation(
//         Summary = "Получить список пользователей.",
//         Description = "Возвращает список пользователей.",
//         OperationId = "User.Get",
//         Tags = new[] { "User" })]
//     public override Task<ActionResult<BaseResponse<PagedList<UserResponse>>>> HandleAsync([FromQuery] GetUsersRequest request, CancellationToken cancellationToken = new CancellationToken())
//     {
//         return base.HandleAsync(request, cancellationToken);
//     }
//     
//     protected override async Task<PagedList<UserResponse>> HandleEndpoint(GetUsersRequest request, CancellationToken cancellationToken = new CancellationToken())
//     {
//         GetUsersDto dto = _mapper.Map<GetUsersDto>(request);
//         var users = await _userRepository.GetUsers(dto);
//         return _mapper.Map<PagedList<UserResponse>>(users);
//     }
// }