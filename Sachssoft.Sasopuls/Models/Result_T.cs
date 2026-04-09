using System;
using System.Globalization;

namespace Sachssoft.Sasofly.Pulse.Models;

/// <summary>
/// Represents the result of an operation, containing a success flag, optional data, and an optional localized error message.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public record Result<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Success { get; init; }

    /// <summary>
    /// The data returned by the operation if successful; otherwise null.
    /// </summary>
    public T? Data { get; init; }

    /// <summary>
    /// The localized error message if the operation failed; otherwise null.
    /// </summary>
    public Localized<string>? Error { get; init; }

    /// <summary>
    /// Parameterless constructor for serialization or default instantiation.
    /// </summary>
    public Result() { }

    /// <summary>
    /// Initializes a new instance of the Result record.
    /// </summary>
    public Result(bool success, T? data = default, Localized<string>? error = null)
    {
        Success = success;
        Data = data;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with the specified data.
    /// </summary>
    public static Result<T> OK(T data) => new() { Success = true, Data = data };

    /// <summary>
    /// Creates a failed result with the specified localized error message.
    /// </summary>
    public static Result<T> Fail(Localized<string> error) => new() { Success = false, Error = error };

    /// <summary>
    /// Returns a culture-invariant string representation of the Result.
    /// </summary>
    public override string ToString()
    {
        var dataStr = Data is IFormattable formattable
            ? formattable.ToString(null, CultureInfo.InvariantCulture)
            : Data?.ToString();

        var errorStr = Error?.Fallback ?? "null";

        return $"Success: {Success}, Data: {dataStr ?? "null"}, Error: {errorStr}";
    }
}