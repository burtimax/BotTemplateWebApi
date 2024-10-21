using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MultipleBotFramework.Extensions;

/// <summary>
/// Extension Methods
/// </summary>
public static class ObjectExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T ThrowIfNull<T>(
        this T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = default
    ) =>
        value ?? throw new ArgumentNullException(parameterName);
    
    public static string ToJson(this object obj)
    {
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        });
    }
}