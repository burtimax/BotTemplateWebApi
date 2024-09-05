using System;
using System.Runtime.CompilerServices;

namespace MultipleBotFramework_Obsolete.Extensions;

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
}