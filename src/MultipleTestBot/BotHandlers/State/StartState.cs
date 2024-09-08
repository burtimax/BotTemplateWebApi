// using Microsoft.EntityFrameworkCore;
// using MultipleBotFramework.Attributes;
// using MultipleBotFramework.Dispatcher.HandlerResolvers;
// using MultipleBotFramework.Extensions.ITelegramApiClient;
// using MultipleBotFramework.Utils.Keyboard;
// using Telegram.BotAPI.AvailableMethods;
// using Telegram.BotAPI.AvailableTypes;
// using Telegram.BotAPI.GettingUpdates;
//
// namespace MultipleTestBot.BotHandlers.State;
//
// [BotHandler(stateName:Name, version: 2.0f)]
// public class StartState : BaseMultipleTestBotState
// {
//     public const string Name = "StartState";
//     
//     public StartState(IServiceProvider serviceProvider) : base(serviceProvider)
//     {
//     }
//
//     public override async Task HandleBotRequest(Update update)
//     {
//         InlineKeyboardBuilder keyboard = new();
//         keyboard.NewRow().Add("Hello", "dsd").Add("Boodbye", "sdas").NewRow().Add("Timon", "dfsd");
//         var mes = await Answer("Hello world!", replyMarkup: keyboard.Build());
//         
//         //var downloadFile = await BotClient.DownloadFileAsync("DQACAgIAAxkBAAJiImazyle97cjKscFhAAG8piVVRQWHpQACDE4AA3egSYtGcncCFkcTNQQ");
//         var downloadFile = await BotClient.DownloadFileAsync("DQACAgIAAxkBAAJiMma2odPJxXBYVHDEGBbm2ryZmIJ7AAL-SgACtXW5SZRMs6_HjvS1NQQ");
//         var mes1 = await BotClient.SendVideoNoteAsync(Chat.ChatId, new InputFile(downloadFile.FileData, downloadFile.FileName));
//         
//         // byte[] file = System.IO.File.ReadAllBytes("C:\\Users\\timof\\Desktop\\Новая папка\\workspace.png");
//         // var message = await BotClient.SendPhotoAsync(Chat.ChatId, new InputFile(file, "workspace"), replyMarkup: new ReplyKeyboardRemove());
//         // var message1 = await BotClient.SendPhotoAsync(Chat.ChatId, "AgACAgIAAxkDAAJhxWayfuIWFY7tAchZ7OSjwhLzD2LpAAJN4zEbzmeQSTkh-aaASpr9AQADAgADeQADNQQ");
//         //
//         // int t = 1;
//     }
// }