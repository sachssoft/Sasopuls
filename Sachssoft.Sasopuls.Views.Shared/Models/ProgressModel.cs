using System;

namespace Sachssoft.Sasopuls.Models
{
    /// <summary>
    /// Represents the progress of an operation, task, or process.
    /// </summary>
    public record ProgressModel
    {
        /// <summary>
        /// Optional localized label or description of the operation.
        /// </summary>
        // Lokalisierter Name oder Beschreibung der Operation
        public Localized<string>? Label { get; init; }

        /// <summary>
        /// Current progress value, e.g., completed steps.
        /// </summary>
        // Aktueller Fortschrittswert
        public double Value { get; init; }

        /// <summary>
        /// Maximum progress value, e.g., total steps.
        /// </summary>
        // Maximaler Fortschrittswert
        public double Max { get; init; } = 100.0;

        /// <summary>
        /// Indicates if the operation is indeterminate (unknown total progress).
        /// </summary>
        // Gibt an, ob der Fortschritt unbestimmt ist
        public bool IsIndeterminate { get; init; } = false;

        /// <summary>
        /// Optional timestamp when progress was last updated.
        /// </summary>
        // Zeitstempel der letzten Aktualisierung
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Returns progress as a percentage between 0 and 100.
        /// </summary>
        public double Percentage => IsIndeterminate ? 0 : Max == 0 ? 0 : (Value / Max) * 100;

        /// <summary>
        /// Returns a string representation of the progress (invariant culture).
        /// </summary>
        public override string ToString() =>
            IsIndeterminate
                ? $"{Label?.Fallback ?? "Operation"}: In progress (indeterminate)"
                : $"{Label?.Fallback ?? "Operation"}: {Value}/{Max} ({Percentage:F2}%)";
    }
}