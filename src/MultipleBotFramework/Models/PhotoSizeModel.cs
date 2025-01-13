namespace MultipleBotFramework.Models;

public class PhotoSizeModel
{
    public string FileId { get; set; }
    public string FileUniqueId { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public long? FileSize { get; set; }
}