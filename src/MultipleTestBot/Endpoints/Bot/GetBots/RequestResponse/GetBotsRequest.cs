using MultipleTestBot.Endpoints.Common.Request;
using MultipleTestBot.Models;

namespace MultipleTestBot.Endpoints.Bot.GetBots.RequestResponse;

public class GetBotsRequest : PaginationRequest, IOrdered
{
    public List<long>? Ids { get; set; }
    public string? Order { get; set; }
}