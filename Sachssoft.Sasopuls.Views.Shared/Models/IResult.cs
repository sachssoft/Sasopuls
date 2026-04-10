namespace Sachssoft.Sasopuls.Models
{
    public interface IResult
    {
        /// <summary>
        /// Indicates whether the operation was successful.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// The data returned by the operation if successful; otherwise null.
        /// </summary>
        object? Data { get; }

        /// <summary>
        /// The localized error message if the operation failed; otherwise null.
        /// </summary>
        Localized<string>? Error { get; }
    }
}