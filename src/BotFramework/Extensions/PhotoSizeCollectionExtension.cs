using System;
using BotFramework.Enums;
using Telegram.Bot.Types;

namespace BotFramework.Extensions;

public static class PhotoSizeCollectionExtension
{
    public static FileBase GetFileByQuality(this PhotoSize[] photoSizes, PhotoQuality quality)
    {
        int qualityIndex = (int) Math.Round(((int)quality) / ((double)PhotoQuality.High) * (photoSizes.Length-1));
        return photoSizes[qualityIndex];
    }
}