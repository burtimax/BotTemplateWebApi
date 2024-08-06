using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultipleBotFrameworkUpgrade.Constants;
using Telegram.BotAPI.AvailableTypes;


namespace MultipleBotFrameworkUpgrade.Utils;

/// <summary>
/// Defines methods to format text in HTML.
/// </summary>
public static class HtmlTextFormatter
{
    /// <inheritdoc/>
    public static string FromEntities(string input, IEnumerable<MessageEntity> entities)
    {
        if (input is null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        if (entities == null)
        {
            throw new ArgumentNullException(nameof(input));
        }
        if (entities.Any())
        {
            var buffer = new StringBuilder();
            var offset = 0;
            string nFsubText;
            string subText;
            foreach (var e in entities)
            {
                if (offset < e.Offset)
                {
                    nFsubText = input.Substring(offset, e.Offset - offset);
                    subText = EncodeHtmlCharacters(nFsubText);
                    buffer.Append(subText);
                    offset = e.Offset;
                }
                subText = input.Substring(offset, e.Length);
                switch (e.Type)
                {
                    case MessageEntityType.Bold:
                        buffer.Append(Bold(subText));
                        break;
                    case MessageEntityType.Italic:
                        buffer.Append(Italic(subText));
                        break;
                    case MessageEntityType.Underline:
                        buffer.Append(Underline(subText));
                        break;
                    case MessageEntityType.Strikethrough:
                        buffer.Append(Strikethrough(subText));
                        break;
                    case MessageEntityType.Spoiler:
                        buffer.Append(Spoiler(subText));
                        break;
                    case MessageEntityType.Code:
                        buffer.Append(Code(subText));
                        break;
                    case MessageEntityType.Pre:
                        buffer.Append(Pre(subText, e.Language));
                        break;
                    case MessageEntityType.TextLink:
                        buffer.Append(TextLink(subText, e.Url!));
                        break;
                    case MessageEntityType.TextMention:
                        buffer.Append(TextMention(subText, e.User!.Id));
                        break;
                    case MessageEntityType.CustomEmoji:
                        buffer.Append(CustomEmoji(subText, e.CustomEmojiId!));
                        break;
                    default:
                        subText = EncodeHtmlCharacters(subText);
                        buffer.Append(subText);
                        break;
                }
                offset += e.Length;
            }
            if (offset < input.Length)
            {
                nFsubText = input.Substring(offset, input.Length - offset);
                subText = EncodeHtmlCharacters(nFsubText);
                buffer.Append(subText);
            }
            return buffer.ToString();
        }
        else
        {
            return EncodeHtmlCharacters(input);
        }
    }

    /// <inheritdoc/>
    public static string Bold(string input)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<b>{text}</b>";
    }

    /// <inheritdoc/>
    public static string Italic(string input)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<i>{text}</i>";
    }

    /// <inheritdoc/>
    public static string Underline(string input)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<u>{text}</u>";
    }

    /// <inheritdoc/>
    public static string Strikethrough(string input)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<s>{text}</s>";
    }

    /// <inheritdoc/>
    public static string Spoiler(string input)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<span class=\"tg-spoiler\">{text}</span>";
    }

    /// <inheritdoc/>
    public static string Code(string input)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<code>{text}</code>";
    }

    /// <inheritdoc/>
    public static string Pre(string input, string? language = null)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);

        return string.IsNullOrEmpty(language)
            ? $"<pre>{text}</pre>"
            : $"<pre><code class=\"language-{language}\">{text}</code></pre>";
    }

    /// <summary> Format text. Text link. </summary>
    /// <param name="input">Input text.</param>
    /// <param name="url">Url.</param>
    /// <param name="parseMode">Style to be applied to the new text.</param>
    /// <param name="useFixer">True, if you want to use the StyleParser.</param>
    /// <returns>Stylized <see cref="string"/></returns>
    public static string TextLink(string input, string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException(nameof(url));
        }

        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<a href=\"{url}\">{text}</a>";
    }

    /// <inheritdoc/>
    public static string TextMention(string input, long userId)
    {
        input ??= string.Empty;
        string text = EncodeHtmlCharacters(input);
        return $"<a href=\"tg://user?id={userId}\">{text}</a>";
    }

    /// <summary>
    /// Format text. Custom Emoji. <br />
    /// </summary>
    /// <remarks>
    /// Custom emoji is not supported with Markdown. Use MarkdownV2 instead.
    /// </remarks>
    /// <param name="customEmojiId">Unique identifier of the custom emoji.</param>
    /// <param name="emoji">The emoji.</param>
    /// <param name="parseMode">Style to be applied to the new text.</param>
    /// <param name="useFixer">True, if you want to use the StyleParser.</param>
    /// <returns>Stylized <see cref="string"/></returns>
    public static string CustomEmoji(string customEmojiId, string emoji)
    {
        if (string.IsNullOrEmpty(customEmojiId))
        {
            throw new ArgumentNullException(nameof(customEmojiId));
        }
        if (string.IsNullOrEmpty(emoji))
        {
            throw new ArgumentNullException(nameof(emoji));
        }

        return $"<tg-emoji emoji-id=\"{customEmojiId}\">{emoji}</tg-emoji>";
    }

    /// <summary>
    /// Replaces symbols that are not part of an HTML tag or entity with HTML entities (&lt; with &amp;lt;, &gt; with &amp;gt; and &amp; with &amp;amp;).
    /// </summary>
    /// <param name="input">Input text.</param>
    /// <returns>String with HTML entities.</returns>
    /// <returns><see cref="string"/></returns>
    public static string EncodeHtmlCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
    }
}