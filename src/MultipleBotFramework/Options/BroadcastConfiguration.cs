namespace MultipleBotFramework.Options;

public class BroadcastConfiguration
{
    public const string Section = "BroadcastMessages";
    
    public string BroadcastDbConnection { get; set; }
    public int MessageDelayMilliseconds { get; set; }
    public bool IsEnabled { get; set; }
}