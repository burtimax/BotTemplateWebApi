namespace MultipleTestBot.Endpoints.Bot.GetBots.RequestResponse;

public class BotResponse
{
    public long Id { get; set; }
    public string Token { get; set; }
    public string? Username { get; set; }

    public string? Description { get; set; }
    
    public string? Comment { get; set; }
}