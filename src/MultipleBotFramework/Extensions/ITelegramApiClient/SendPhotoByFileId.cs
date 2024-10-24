// using System.Threading;
// using System.Threading.Tasks;
// using Telegram.BotAPI;
// using Telegram.Bot.Requests;
// 
//
// namespace BotFramework.Extensions;
//
// public static class SendPhotoByFileId
// {
//     public static async Task<Message> SendPhotoAsync(
//         this ITelegramBotClient botClient,
//         SendPhotoRequest request,
//         CancellationToken cancellationToken = default
//     ) =>
//         await botClient.ThrowIfNull()
//             .MakeRequestAsync(request, cancellationToken)
//             .ConfigureAwait(false);
// }