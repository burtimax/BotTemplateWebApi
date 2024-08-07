using System;
using System.Linq;
using Telegram.BotAPI.AvailableTypes;
using NullReferenceException = System.NullReferenceException;

namespace MultipleBotFrameworkUpgrade.Models;

public class DownloadedTelegramFile
{
    public File FileInfo { get; set; }
    public byte[] FileData { get; set; }

    public string FileName { get; set; }
    public string FileNameWithExtension { get; set; }
    public string FileExtension { get; set; }
    
    public DownloadedTelegramFile(File fileInfo, byte[] fileData)
    {
        FileInfo = fileInfo;
        FileData = fileData;
        if (string.IsNullOrEmpty(fileInfo.FilePath)) throw new NullReferenceException("FilePath is null!");
        FilePath path = new("/" + fileInfo.FilePath);
        FileNameWithExtension = path.FileName;
        FileExtension = path.Extension;
        FileName = path.FileNameWithoutExtension;
    }
}