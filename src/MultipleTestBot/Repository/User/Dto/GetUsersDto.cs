using MultipleTestBot.Models;

namespace MultipleTestBot.Repository.User.Dto;

public class GetUsersDto : Pagination, IOrdered
{
    public List<long>? Ids { get; set; }
    public List<long>? BotIds { get; set; }
    public string? Order { get; set; }
}