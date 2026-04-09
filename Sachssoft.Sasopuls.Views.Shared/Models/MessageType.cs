namespace Sachssoft.Sasofly.Pulse.Models
{
    /// <summary>
    /// Represents the category or severity of a message, used for UI, logging, or system notifications.
    /// </summary>
    public enum MessageType
    {
        /// <summary>General informational message.</summary>
        // Allgemeine Informationsnachricht
        Informational,

        /// <summary>Positive message indicating successful operation.</summary>
        // Erfolgsnachricht, zeigt eine erfolgreich abgeschlossene Aktion an
        Success,

        /// <summary>Warning message, requires user attention.</summary>
        // Warnung, erfordert Aufmerksamkeit des Benutzers
        Warning,

        /// <summary>Error message indicating a failed operation.</summary>
        // Fehlermeldung, zeigt einen Fehler an
        Error,

        /// <summary>Critical system failure requiring immediate attention.</summary>
        // Kritischer Systemfehler, sofortige Handlung erforderlich
        Critical,

        /// <summary>Debug message for developers or logging purposes.</summary>
        // Debug-Nachricht für Entwickler oder Log-Zwecke
        Debug,

        /// <summary>System notification or background event.</summary>
        // Systembenachrichtigung oder Hintergrundereignis
        Notification,

        /// <summary>Message containing a question or prompt for the user.</summary>
        // Frage oder Eingabeaufforderung an den Benutzer
        Question,

        /// <summary>Message providing a hint, suggestion, or contextual help.</summary>
        // Hinweis oder Tipp für den Benutzer
        Hint,

        /// <summary>Message containing instructions or guidance for the user.</summary>
        // Anweisung oder Anleitung für den Benutzer
        Guidance,

        /// <summary>Message that requires user confirmation or acknowledgment.</summary>
        // Nachricht, die Bestätigung oder Rückmeldung des Benutzers benötigt
        Confirmation,

        /// <summary>Informational tip or hint to assist user actions.</summary>
        // Informations-Tipp zur Unterstützung von Benutzeraktionen
        Tip,

        /// <summary>Message that indicates a pending operation.</summary>
        // Nachricht über eine ausstehende Operation
        Pending,

        /// <summary>Message about ongoing operation progress.</summary>
        // Nachricht über den Fortschritt einer laufenden Operation
        Progress,

        /// <summary>Message that indicates cancellation of an operation.</summary>
        // Nachricht über eine abgebrochene Operation
        Cancelled,

        /// <summary>Message indicating timeout occurred.</summary>
        // Nachricht über eine Zeitüberschreitung
        Timeout,

        /// <summary>Message indicating deprecated feature usage.</summary>
        // Nachricht über die Nutzung einer veralteten Funktion
        Deprecated
    }
}