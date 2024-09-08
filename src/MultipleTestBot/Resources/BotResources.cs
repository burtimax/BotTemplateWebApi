namespace MultipleTestBot.Resources;

public partial class BotResources
{
    public long IntroductionSavedMessageId { get; set; }
    public string Introduction { get; set; }
    public string NoSupportedMessage { get; set; }
    public string SendMeNoTextMessage { get; set; }
    public string NeedApprove { get; set; }
    public string ReplyForYourMessage { get; set; }
    public string MessageApprovedByModerator { get; set; }
    public string MessageDeclinedByModerator { get; set; }
    public string MessageSaved { get; set; }
    public string ButtonNext { get; set; }
    public string ButtonApprove { get; set; }

    public string ButtonDecline { get; set; }


    public const string ButtonApproveCallbackKey = "moder_approve";
    public const string ButtonDeclineCallbackKey = "moder_decline";
}