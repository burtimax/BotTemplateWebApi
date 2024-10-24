using Microsoft.AspNetCore.Mvc.Filters;
using MultipleTestBot.Models;

namespace MultipleTestBot.Repository.Bot.Dto;

public class GetBotsDto : Pagination, IOrdered
{
    public List<long>? Ids { get; set; }
    
    /// <inheritdoc />
    public string? Order { get; set; }
}