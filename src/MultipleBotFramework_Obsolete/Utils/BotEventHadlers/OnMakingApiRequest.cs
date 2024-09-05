using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MultipleBotFramework_Obsolete.Utils.BotEventHadlers;

public class OnMakingApiRequest
{
    internal static int BoundRequestInSecond = 30;
    private static object lockObj = new();
    private static Queue<long> RequestTimestamps = new();
    
    public static async ValueTask Handler(
        ITelegramBotClient botClient,
        ApiRequestEventArgs args,
        CancellationToken cancellationToken = default)
    {
        // Прописываем логику, чтобы бот не выходил за ограничения по запросам в секунду.
        
        long timestamp = DateTime.UtcNow.Ticks;

        if (RequestTimestamps.Count <= BoundRequestInSecond)
        {
            RequestTimestamps.Enqueue(timestamp);
            return;
        }

        var topQueue = RequestTimestamps.Peek();

        if ((timestamp - topQueue) < TimeSpan.TicksPerSecond)
        {
            var dif = (timestamp - topQueue) / TimeSpan.TicksPerMillisecond + 1;
            
            // Нужно ждать, чтобы не выйти в ограничения.
            lock (lockObj)
            {
                Task.Delay((int) dif).GetAwaiter();
            }
        }

        RequestTimestamps.Dequeue();
        RequestTimestamps.Enqueue(timestamp);
    }
    
}