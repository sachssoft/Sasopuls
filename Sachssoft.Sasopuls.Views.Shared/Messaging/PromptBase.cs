using System;

namespace Sachssoft.Sasopuls.Messaging
{
    // Prompt<TInput, TOutput>
    // ======================
    // Zweck:
    //   Leichte, framework-unabhängige Alternative zu
    //   ReactiveUI.Interaction<TInput, TOutput>.
    //
    // Vergleich zu ReactiveUI:
    //   Interaction.Handle(input)        -> Prompt.ExecuteAsync(input)
    //   Interaction.RegisterHandler(...) -> Prompt.AddHandler(...)
    //
    // Architektur:
    //   - ViewModel kennt keine View
    //   - View registriert den Handler
    //   - Kommunikation erfolgt ausschließlich async
    //
    // Design-Entscheidungen:
    //   - keine try/catch-Blöcke (Fehler propagieren bewusst)
    //   - exakt ein Handler (deterministisch)

    /// <summary>
    /// Minimaler Prompt für MVVM: fragt etwas ab, gibt Antwort zurück.
    /// Optionaler Default-Wert wird genutzt, falls kein Handler registriert ist.
    /// Alle Exception-Meldungen sind auf Englisch.
    /// </summary>
    public abstract class PromptBase
    {
        internal PromptBase() { }

        internal static void ThrowRegistrationException(object? handler)
        {
            _ = handler ?? throw new ArgumentNullException(nameof(handler), "Handler cannot be null.");
        }

        internal static Exception ThrowExecuteException()
        {
            return new InvalidOperationException("No handler registered and no default value set.");
        }
    }
}
