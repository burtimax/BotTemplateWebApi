using System;
using System.Collections.Generic;
using MultipleBotFramework.Base;
using MultipleBotFramework.Models;

namespace MultipleBotFramework.Quartz.Jobs.Notification;

public class BotNotificationTask
{
    public long BotId { get; set; }
    public List<long> ChatIds { get; set; } = new List<long>();
    public Delegates.BotNotificationAction Action { get; set; }
}