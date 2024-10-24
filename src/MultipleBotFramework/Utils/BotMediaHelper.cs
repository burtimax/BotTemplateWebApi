using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using MultipleBotFramework.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using File = System.IO.File;

namespace MultipleBotFramework.Utils
{
    public class BotMediaHelper
    {
        /// <summary>
        /// Скачать и сохранить файл из Telegram серверов в директорию.
        /// </summary>
        /// <param name="botClient">API клиент.</param>
        /// <param name="file">Данные о файле.</param>
        /// <param name="fp">Полный путь к файлу.</param>
        public static async Task DownloadAndSaveTelegramFileAsync(ITelegramBotClient botClient, string fileId, FilePath fp)
        {
            using (FileStream fs = new FileStream(fp, FileMode.Create))
            {
                Telegram.BotAPI.AvailableTypes.File fileInfo = await botClient.GetFileAsync(fileId);
                
                using (var client = new HttpClient()) // WebClient
                {
                    byte[] file = await client.GetByteArrayAsync(fileInfo.FilePath);
                    fs.Position = 0;
                    await fs.WriteAsync(file);
                }
            }
        }
        
        private async Task<HttpResponseMessage> GetResponseAsync(
            HttpClient httpClient,
            string fileUri,
            CancellationToken cancellationToken)
        {
            HttpResponseMessage? httpResponse;
            try
            {
                httpResponse = await httpClient
                    .GetAsync(
                        requestUri: fileUri,
                        completionOption: HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken: cancellationToken
                    )
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (TaskCanceledException exception)
            {
                if (cancellationToken.IsCancellationRequested) { throw; }

                throw new BotRequestException(
                    errorCode: 500,
                    description: "Request timed out",
                    null
                );
            }
            catch (Exception exception)
            {
                throw new BotRequestException(
                    description: "Exception during file download",
                    errorCode: 500, 
                    parameters:null
                );
            }

            return httpResponse;
        }

        /// <summary>
        /// Сохранить файл на диске.
        /// </summary>
        /// <param name="filePath">Полный путь к файлу.</param>
        /// <param name="ms">Данные файла.</param>
        public static async Task SaveFileAsync(FilePath filePath, MemoryStream ms)
        {
            if (ms is null || ms.Length == 0) throw new ArgumentNullException(nameof(ms));
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                ms.Position = 0;
                await fs.WriteAsync(ms.GetBuffer());
            }
        }

        /// <summary>
        /// Получить файл.
        /// </summary>
        /// <param name="filePath">Путь к файлу.</param>
        /// <returns></returns>
        public static async Task<MemoryStream> GetFileByPathAsync(FilePath filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentNullException(nameof(filePath));
            
            MemoryStream ms = new(await File.ReadAllBytesAsync(filePath));
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Скачать файл из серверов Telegram по ИД.
        /// </summary>
        /// <param name="botClient">API клиент.</param>
        /// <param name="fileId">ИД файла.</param>
        /// <returns>Данные по файлу.</returns>
        public static async Task<( Telegram.BotAPI.AvailableTypes.File fileInfo, MemoryStream fileData)> GetFileFromTelegramAsync(ITelegramBotClient botClient, string fileId)
        {
            if (botClient == null) throw new ArgumentNullException(nameof(botClient));
            if (string.IsNullOrEmpty(fileId)) throw new ArgumentNullException(nameof(fileId));
            MemoryStream ms = new MemoryStream();
            
            Telegram.BotAPI.AvailableTypes.File fileInfo = await botClient.GetFileAsync(fileId);
            
            using (var client = new HttpClient()) // WebClient
            {
                byte[] file = await client.GetByteArrayAsync(fileInfo.FilePath);
                ms.Position = 0;
                await ms.WriteAsync(file);
            }

            return (fileInfo, ms);
        }

        // TODO REFACTOR
        /// <summary>
        /// Получить фото из Telegram.
        /// </summary>
        /// <param name="botClient">API клиент.</param>
        /// <param name="requiredQuality">Требуемое качество фото.</param>
        /// <param name="photoSizes">Фото размеры. <see cref="Message.Photo"/></param>
        /// <returns>Данные по фото.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        // public static Task<(Telegram.Bot.Types.File fileInfo, MemoryStream fileData)> GetPhotoFromTelegramAsync(
        //     ITelegramBotClient botClient, PhotoQuality requiredQuality, PhotoSize[] photoSizes)
        // {
        //     if (botClient is null) throw new ArgumentNullException(nameof(botClient));
        //     if (photoSizes is null || photoSizes.Any() == false) throw new ArgumentNullException(nameof(photoSizes));
        //     
        //     int index = (int)requiredQuality - 1;
        //     if (index >= photoSizes.Length) index = photoSizes.Length - 1;
        //     return GetFileFromTelegramAsync(botClient, photoSizes[index].FileId);
        // }
    }
}
