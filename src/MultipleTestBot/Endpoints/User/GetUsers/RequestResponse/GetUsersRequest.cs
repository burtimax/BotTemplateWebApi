using MultipleTestBot.Endpoints.Common.Request;
using MultipleTestBot.Models;

namespace MultipleTestBot.Endpoints.User.GetUsers.RequestResponse;

public class GetUsersRequest : PaginationRequest, IOrdered
{
    public List<long>? Ids { get; set; }
    public List<long>? BotIds { get; set; }
    public string? Order { get; set; }
}