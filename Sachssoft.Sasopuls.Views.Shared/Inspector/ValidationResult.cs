using System;

namespace Sachssoft.Sasopuls.Inspector
{
    public readonly struct ValidationResult
    {
        public bool IsValid { get; }

        public object? SuggestedValue { get; }

        public Exception? Exception { get; }

        public ValidationResult(
            bool isValid,
            object? suggestedValue = null,
            Exception? exception = null)
        {
            IsValid = isValid;
            SuggestedValue = suggestedValue;
            Exception = exception;
        }

        public static ValidationResult Success()
            => new(true);

        public static ValidationResult Fail(Exception exception, object? suggestion = null)
            => new(false, suggestion, exception ?? throw new ArgumentNullException(nameof(exception)));
    }
}