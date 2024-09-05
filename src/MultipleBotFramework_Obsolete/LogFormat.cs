namespace MultipleBotFramework_Obsolete;

public class LogFormat
{
    /// <summary>
    /// {1} - Update ID;
    /// {2} - User identifiers;
    /// {3} - Chat identifiers;
    /// {4} - Chat state;
    /// {5} - Update type;
    /// </summary>
    public const string ReceiveUpdate = "NEW UPDATE: Id [{1}]; User [{2}]; Chat [{3}]; State [{4}]; Update [{5}];";
    
    /// <summary>
    /// {1} - Update Id;
    /// </summary>
    public const string ProcessedUpdate = "END UPDATE: Id [{1}]";
    
    /// <summary>
    /// {1} - Update Id;
    /// {2} - Exception message;
    /// </summary>
    public const string ExceptionUpdate = "ERROR UPDATE: Id [{1}]; Message [{2}]";
}