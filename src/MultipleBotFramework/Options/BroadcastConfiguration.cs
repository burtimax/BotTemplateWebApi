namespace MultipleBotFramework.Options;

public class BroadcastConfiguration
{
    public const string Section = "BroadcastMessages";
    public int MessageDelayMilliseconds { get; set; }
    public bool IsEnabled { get; set; }
}