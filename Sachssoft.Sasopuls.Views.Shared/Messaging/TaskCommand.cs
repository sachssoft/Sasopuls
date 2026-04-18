using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Sachssoft.Sasopuls.Messaging
{
    public sealed class TaskCommand : CommandBase
    {
        private readonly Func<object?, Task> _executeAsync;
        private readonly Func<object?, bool>? _canExecute;
        private readonly ICommandRule? _rule;

        private int _isRunning;

        public bool CanExecuteAlwaysIfNull { get; }
        public bool IsRunning => _isRunning == 1;

        // -------------------------
        // Sync wrapper constructors
        // -------------------------

        public TaskCommand(
            Func<Task> executeAsync,
            Func<bool>? canExecute = null,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
            : this(
                executeAsync: (param) => executeAsync(),
                canExecute: canExecute != null ? ((param) => canExecute()) : null,
                canExecuteAlwaysIfNull: canExecuteAlwaysIfNull,
                owner: owner)
        { }

        public TaskCommand(
            Func<object?, Task> executeAsync,
            Func<object?, bool>? canExecute = null,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
            CanExecuteAlwaysIfNull = canExecuteAlwaysIfNull;

            if (owner != null)
                owner.PropertyChanged += (_, __) => RaiseCanExecuteChanged();
        }

        // -------------------------
        // Rule-based constructors (FIXED)
        // -------------------------

        public TaskCommand(
            Func<Task> executeAsync,
            ICommandRule rule,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
            : this(
                executeAsync: (param) => executeAsync(),
                rule: rule,
                canExecuteAlwaysIfNull: canExecuteAlwaysIfNull,
                owner: owner)
        { }

        public TaskCommand(
            Func<object?, Task> executeAsync,
            ICommandRule rule,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _rule = rule ?? throw new ArgumentNullException(nameof(rule));

            _canExecute = rule.CanExecute;
            CanExecuteAlwaysIfNull = canExecuteAlwaysIfNull;

            // 🔥 reactive binding (Rule → Command)
            _rule.CanExecuteChanged += OnRuleChanged;

            if (owner != null)
                owner.PropertyChanged += (_, __) => RaiseCanExecuteChanged();
        }

        // -------------------------
        // Rule event handler
        // -------------------------

        private void OnRuleChanged(object? sender, EventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        // -------------------------
        // Execution logic
        // -------------------------

        public override bool CanExecute(object? parameter)
        {
            if (_canExecute == null && CanExecuteAlwaysIfNull)
                return true;

            if (_canExecute == null)
                return true;

            return !IsRunning && _canExecute(parameter);
        }

        public override async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter).ConfigureAwait(false);
        }

        public async Task ExecuteAsync(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            Interlocked.Exchange(ref _isRunning, 1);
            RaiseCanExecuteChanged();

            try
            {
                await _executeAsync(parameter).ConfigureAwait(true);
            }
            finally
            {
                Interlocked.Exchange(ref _isRunning, 0);
                RaiseCanExecuteChanged();
            }
        }

        // -------------------------
        // Manual refresh
        // -------------------------

        public void RefreshCanExecute()
            => RaiseCanExecuteChanged();
    }
}