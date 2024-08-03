using System.IO;

namespace MultipleBotFramework.Utils;

public class StreamHelper
{
    /// <summary>
    /// Сгенерировать stream из string.
    /// </summary>
    /// <param name="s">Строка данных.</param>
    /// <returns></returns>
    public static Stream GenerateStreamFromString(string s)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }
}