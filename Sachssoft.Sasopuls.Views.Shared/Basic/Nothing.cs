namespace Sachssoft.Sasofly.Pulse
{
    // Repräsentiert einen Platzhalter für "kein Wert".
    // 
    // Dieser Typ kann als generischer Rückgabewert für Methoden, Commands oder Tasks verwendet werden,
    // die keinen sinnvollen Wert liefern.
    // 
    // Anders als System.Reactive.Unit ist dies:
    // - Vollständig intern und ohne externe Bibliotheken
    // - AOT- und Trimming-freundlich
    // - Singleton-basiert, um unnötige Zuweisungen zu vermeiden
    //
    // Die Verwendung von Nothing statt System.Reactive.Unit stellt sicher, dass unsere SharedUI-Library
    // leichtgewichtig bleibt und keine Reflection-abhängigen Bibliotheken wie Rx.NET benötigt.

    /// <summary>
    /// Represents a singleton placeholder for "no value".
    /// 
    /// This type can be used as a generic return type for methods, commands, or tasks that do not produce a meaningful value.
    /// Unlike <c>System.Reactive.Unit</c>, this implementation is:
    /// - Fully internal and self-contained (no external library dependency)
    /// - AOT-friendly and trimming-safe
    /// - Singleton-based to avoid unnecessary allocations
    /// 
    /// Using <see cref="Nothing"/> instead of System.Reactive.Unit ensures that our SharedUI library
    /// remains lightweight and free of dependencies on Rx.NET, which is reflection-heavy and may cause
    /// issues in AOT or trimming scenarios (e.g., iOS, WebAssembly, or publish trimming).
    /// </summary>
    public readonly struct Nothing
    {
        /// <summary>
        /// The single, shared instance of <see cref="Nothing"/>.
        /// Always use this instance instead of creating new ones.
        /// </summary>
        public static readonly Nothing Instance = new Nothing();
    }
}
