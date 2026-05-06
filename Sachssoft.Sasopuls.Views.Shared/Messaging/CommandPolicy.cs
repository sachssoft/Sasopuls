using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sachssoft.Sasopuls.Messaging
{
    public sealed class CommandPolicy : ICommandRule, IInvalidateable
    {
        private readonly Func<object?, bool>? _canExecute;
        private readonly CommandDispatcher? _dispatcher;
        private readonly List<CommandPolicy>? _sources;

        private CommandPolicy(
            Func<object?, bool>? canExecute,
            CommandDispatcher? dispatcher = null,
            List<CommandPolicy>? sources = null)
        {
            _canExecute = canExecute;
            _dispatcher = dispatcher;
            _sources = sources;

            // Event propagation (Rule graph)
            if (_sources != null)
            {
                foreach (var s in _sources)
                    s.CanExecuteChanged += OnSourceChanged;
            }
        }

        public event EventHandler? CanExecuteChanged;

        // -------------------------
        // Notify
        // -------------------------
        public void Invalidate()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        // -------------------------
        // Core evaluation
        // -------------------------
        public bool CanExecute(object? parameter = null)
            => _canExecute?.Invoke(parameter) ?? true;

        // -------------------------
        // Event handling
        // -------------------------

        private void OnSourceChanged(object? sender, EventArgs e)
            => Invalidate();

        // -------------------------
        // Dispatcher hook (optional)
        // -------------------------
        public void NotifyChanged()
            => _dispatcher?.NotifyChanged();

        // -------------------------
        // Factory methods
        // -------------------------
        public static CommandPolicy Create(Func<bool>? canExecute)
            => new CommandPolicy(canExecute != null ? (_ => canExecute()) : null);

        public static CommandPolicy Create(Func<object?, bool>? canExecute)
            => new CommandPolicy(canExecute);

        public static CommandPolicy Create(
            Func<object?, bool>? canExecute,
            CommandDispatcher dispatcher)
            => new CommandPolicy(canExecute, dispatcher);

        // -------------------------
        // AND (Combine)
        // -------------------------
        public CommandPolicy Combine(params CommandPolicy[] others)
        {
            var all = new List<CommandPolicy>(others.Length + 1)
            {
                this
            };
            all.AddRange(others);

            return new CommandPolicy(
                p =>
                {
                    for (int i = 0; i < all.Count; i++)
                        if (!all[i].CanExecute(p))
                            return false;

                    return true;
                },
                _dispatcher,
                all
            );
        }

        // -------------------------
        // OR (Any)
        // -------------------------
        public CommandPolicy Any(params CommandPolicy[] others)
        {
            var all = new List<CommandPolicy>(others.Length + 1)
            {
                this
            };
            all.AddRange(others);

            return new CommandPolicy(
                p =>
                {
                    for (int i = 0; i < all.Count; i++)
                        if (all[i].CanExecute(p))
                            return true;

                    return false;
                },
                _dispatcher,
                all
            );
        }

        // -------------------------
        // NOT (Invert)
        // -------------------------
        public CommandPolicy Invert()
        {
            var sources = new List<CommandPolicy> { this };

            return new CommandPolicy(
                p => !CanExecute(p),
                _dispatcher,
                sources
            );
        }

        // -------------------------
        // Implicit conversion
        // -------------------------
        public static implicit operator CommandPolicy(Func<object?, bool> func)
            => new CommandPolicy(func);

        // -------------------------
        // Constants
        // -------------------------
        public static readonly CommandPolicy Always = new CommandPolicy(_ => true);
        public static readonly CommandPolicy Never = new CommandPolicy(_ => false);
    }
}