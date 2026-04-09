using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Sachssoft.Sasofly.Pulse.Messaging
{
    /// <summary>
    /// Async Command – TaskCommand
    /// ===========================
    /// Führt async Code aus, blockiert Re-Entrancy, optional CanExecute
    /// </summary>
    public sealed class TaskCommand : CommandBase
    {
        private readonly Func<object?, Task> _executeAsync;
        private readonly Func<object?, bool>? _canExecute;
        private int _isRunning; // 0 = false, 1 = true

        /// <summary>
        /// Wenn true und _canExecute == null, liefert CanExecute automatisch true
        /// </summary>
        public bool CanExecuteAlwaysIfNull { get; }

        /// <summary>
        /// Gibt an, ob der Command aktuell läuft
        /// </summary>
        public bool IsRunning => _isRunning == 1;

        // Ohne Parameter
        public TaskCommand(
            Func<Task> executeAsync,
            Func<bool>? canExecute = null,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
            : this(
                  executeAsync: (param) => executeAsync(),
                  canExecute: canExecute != null ? ((param) => canExecute()) : null,
                  canExecuteAlwaysIfNull: canExecuteAlwaysIfNull,
                  owner: owner
            )
        { }

        // Mit Parameter
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
            {
                owner.PropertyChanged += (_, __) => RaiseCanExecuteChanged();
            }
        }

        // Ohne Parameter
        public TaskCommand(
            Func<Task> executeAsync,
            CommandCondition condition,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
            : this(
                  executeAsync: (param) => executeAsync(),
                  condition: condition,
                  canExecuteAlwaysIfNull: canExecuteAlwaysIfNull,
                  owner: owner
            )
        { }

        // Mit Parameter
        public TaskCommand(
            Func<object?, Task> executeAsync,
            CommandCondition condition,
            bool canExecuteAlwaysIfNull = true,
            INotifyPropertyChanged? owner = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = condition != null ? condition.CanExecute : throw new ArgumentNullException(nameof(condition));
            CanExecuteAlwaysIfNull = canExecuteAlwaysIfNull;

            if (owner != null)
            {
                owner.PropertyChanged += (_, __) => RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Prüft, ob der Command ausführbar ist
        /// </summary>
        public override bool CanExecute(object? parameter)
        {
            if (_canExecute == null && CanExecuteAlwaysIfNull)
                return true;

            if (_canExecute == null)
                return true; // Standard fallback

            // Command blockiert während Ausführung
            return !IsRunning && _canExecute(parameter);
        }

        /// <summary>
        /// ICommand-Interface (async void hier korrekt)
        /// </summary>
        public override async void Execute(object? parameter)
        {
            await ExecuteAsync(parameter).ConfigureAwait(false);
        }

        /// <summary>
        /// Async-Variante von Execute
        /// </summary>
        public async Task ExecuteAsync(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            Interlocked.Exchange(ref _isRunning, 1);
            RaiseCanExecuteChanged();

            await _executeAsync(parameter).ConfigureAwait(true); // UI-Kontext beibehalten

            Interlocked.Exchange(ref _isRunning, 0);
            RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Manuelles Refresh für CanExecute
        /// </summary>
        public void RefreshCanExecute() => RaiseCanExecuteChanged();
    }
}