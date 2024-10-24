namespace MultipleBotFramework_Obsolete.Models;

/// <summary>
/// Сообщение для копирования из какого либо чата в Telegram.
/// </summary>
public class CopyMessageInfo
{
    /// <summary>
    /// Из какого чата сообщение.
    /// </summary>
    public long FromChatId { get; set; }
    
    /// <summary>
    /// ИД сообщения.
    /// </summary>
    public int MessageId { get; set; }
}