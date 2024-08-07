using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MultipleBotFrameworkUpgrade.Db;
using MultipleBotFrameworkUpgrade.Db.Entity;
using MultipleBotFrameworkUpgrade.Models;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using File = Telegram.BotAPI.AvailableTypes.File;

namespace MultipleBotFrameworkUpgrade.Extensions.ITelegramApiClient;

public static partial class ITelegramBotClientExtensions
{
    public static async Task<DownloadedTelegramFile> DownloadFileAsync(
        this ITelegramBotClient botClient,
        string fileId,
        CancellationToken cancellationToken = default)
    {
        File fileData = await botClient.GetFileAsync(fileId);

        if (fileData is null) throw new Exception("Exception during get file info");

        string filePath = fileData.FilePath;
        
        if (string.IsNullOrWhiteSpace(filePath) || filePath.Length < 2)
        {
            throw new ArgumentException(message: "Invalid file path", paramName: nameof(filePath));
        }
        
        var fileUri = $"{botClient.Options.ServerAddress}/file/bot{botClient.Options.BotToken}/{filePath}";
        using (HttpClient _httpClient = new())
        {
            using HttpResponseMessage httpResponse = await GetResponseAsync(
                httpClient: _httpClient,
                fileUri: fileUri,
                cancellationToken: cancellationToken
            ).ConfigureAwait(false);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception("Exception during file download");
            }

            if (httpResponse.Content is null)
            {
                throw new Exception("Response doesn't contain any content");
                // throw new RequestException(
                //     message: "Response doesn't contain any content",
                //     httpResponse.StatusCode
                // );
            }

            try
            {
                byte[] bytes = await httpResponse.Content.ReadAsByteArrayAsync()
                    .ConfigureAwait(false);
                
                return new(fileData, bytes);
            }
            catch (Exception exception)
            {
                throw new Exception("Exception during file download");
                // throw new RequestException(
                //     message: "Exception during file download",
                //     httpResponse.StatusCode,
                //     exception
                // );
            }
        }

        [MethodImpl(methodImplOptions: MethodImplOptions.AggressiveInlining)]
        static async Task<HttpResponseMessage> GetResponseAsync(
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

                throw;
                // throw new RequestException(
                //     message: "Request timed out",
                //     innerException: exception
                // );
            }
            catch (Exception exception)
            {
                throw;
                // throw new RequestException(
                //     message: "Exception during file download",
                //     innerException: exception
                // );
            }

            return httpResponse;
        }
    }
    
}