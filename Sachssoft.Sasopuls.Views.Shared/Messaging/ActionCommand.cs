using System;
using System.Threading;

namespace Sachssoft.Sasopuls.Messaging
{
    public sealed class ActionCommand : CommandBase
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private readonly ICommandRule? _rule;

        private int _isRunning;

        public bool IsRunning => _isRunning == 1;

        // Ohne Parameter
        public ActionCommand(
            Action execute,
            Func<bool>? canExecute = null)
            : this(
                execute: (param) => execute(),
                canExecute: canExecute != null ? ((param) => canExecute()) : null
            )
        { }

        // Mit Parameter (Func)
        public ActionCommand(
            Action<object?> execute,
            Func<object?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Ohne Parameter (Rule)
        public ActionCommand(
            Action execute,
            ICommandRule rule)
            : this(
                execute: (param) => execute(),
                rule: rule
            )
        { }

        // Mit Rule (WICHTIG: nur intern erweitert)
        public ActionCommand(
            Action<object?> execute,
            ICommandRule rule)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));

            _canExecute = rule.CanExecute;

            // 🔥 reactive binding (kein Dispose nötig, gleiche Lifetime angenommen)
            _rule.CanExecuteChanged += OnRuleChanged;
        }

        // 🔥 neu: zentraler Handler (keine API Änderung)
        private void OnRuleChanged(object? sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        public override bool CanExecute(object? parameter)
        {
            if (IsRunning)
                return false;

            // Rule hat Vorrang, falls vorhanden
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public override void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            Interlocked.Exchange(ref _isRunning, 1);
            RaiseCanExecuteChanged();

            try
            {
                _execute(parameter);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
                RaiseCanExecuteChanged();
            }
        }
    }
}