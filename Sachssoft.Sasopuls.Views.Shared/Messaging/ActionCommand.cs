using System;
using System.Windows.Input;

namespace Sachssoft.Sasopuls.Messaging
{
    /// <summary>
    /// Sync Command – ActionCommand
    /// ============================
    /// Führt synchronen Code aus.
    /// Implementiert ICommand für UI-Bindings.
    /// </summary>
    public sealed class ActionCommand : CommandBase
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            private set
            {
                if (_isRunning == value) return;
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        // Ohne Parameter
        public ActionCommand(
            Action execute,
            Func<bool>? canExecute = null)
            : this(
                 execute: (param) => execute(),
                 canExecute: canExecute != null ? ((param) => canExecute()) : null
            )
        { }

        // Mit Parameter
        public ActionCommand(
            Action<object?> execute,
            Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Ohne Parameter
        public ActionCommand(
            Action execute,
            ICommandRule rule)
            : this(
                 execute: (param) => execute(),
                 rule: rule
            )
        { }

        // Mit Parameter
        public ActionCommand(
            Action<object?> execute,
            ICommandRule rule)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = rule != null ? rule.CanExecute : throw new ArgumentNullException(nameof(rule));
        }

        public override bool CanExecute(object? parameter) => !IsRunning && (_canExecute?.Invoke(parameter) ?? true);

        public override void Execute(object? parameter)
        {
            if (!CanExecute(parameter)) return;

            IsRunning = true;
            try
            {
                _execute(parameter);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
