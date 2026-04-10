using System;

namespace Sachssoft.Sasopuls.Models
{

    /// <summary>
    /// Represents a message to display to the user or log, with type and optional localized content and formatting.
    /// </summary>
    public record MessageModel
    {
        /// <summary>
        /// The localized message content or template.
        /// </summary>
        // Lokalisierter Nachrichtentext oder Template mit Platzhaltern
        public Localized<string> Message { get; init; }

        /// <summary>
        /// Optional parameters to fill the message template.
        /// </summary>
        // Parameter für Platzhalter in der Nachricht
        public object?[]? Parameters { get; init; }

        /// <summary>
        /// The type of the message (Info, Success, Warning, Error, etc.).
        /// </summary>
        // Typ der Nachricht
        public MessageType Type { get; init; }

        /// <summary>
        /// Optional timestamp when the message was created.
        /// </summary>
        // Zeitstempel der Nachricht
        public DateTime Timestamp { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Initializes a new instance of MessageModel with specified message, type, and optional parameters.
        /// </summary>
        public MessageModel(Localized<string> message, MessageType type, params object[]? parameters)
        {
            Message = message;
            Type = type;
            Parameters = parameters;
        }

        /// <summary>
        /// Returns the formatted message as a string (invariant culture), safely handling null fallback.
        /// </summary>
        public string GetFormattedMessage()
        {
            var template = Message?.Fallback ?? string.Empty; // Nullsicher
            if (Parameters != null && Parameters.Length > 0)
            {
                try
                {
                    return string.Format(System.Globalization.CultureInfo.InvariantCulture, template, Parameters);
                }
                catch (FormatException)
                {
                    // Falls die Parameter nicht zum Template passen, Fallback als unformatierten String zurückgeben
                    return template;
                }
            }
            return template;
        }

        /// <summary>
        /// Returns a culture-invariant string representation of the message, including timestamp and type.
        /// </summary>
        public override string ToString() =>
            $"[{Timestamp:u}] {Type}: {GetFormattedMessage()}";
    }
}