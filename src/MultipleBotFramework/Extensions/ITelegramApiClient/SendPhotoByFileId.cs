// using System.Threading;
// using System.Threading.Tasks;
// using Telegram.Bot;
// using Telegram.Bot.Requests;
// using Telegram.Bot.Types;
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