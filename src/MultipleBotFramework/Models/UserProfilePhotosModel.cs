using System.Collections.Generic;
using Telegram.BotAPI.AvailableTypes;

namespace MultipleBotFramework.Models;

public class UserProfilePhotosModel
{
    /// <summary>Total number of profile pictures the target user has</summary>
    public int TotalCount { get; set; }

    /// <summary>Requested profile pictures (in up to 4 sizes each)</summary>
    public IEnumerable<IEnumerable<PhotoSizeModel>> Photos { get; set; }
}