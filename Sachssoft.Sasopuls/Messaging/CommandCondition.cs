using System;

namespace Sachssoft.Sasofly.Pulse.Messaging
{
    public sealed class CommandCondition
    {
        private readonly Func<object?, bool>? _canExecute;

        private CommandCondition(Func<object?, bool>? canExecute)
            => _canExecute = canExecute;

        // Ohne Parameter
        public static CommandCondition Create(Func<bool>? canExecute)
        {
            return new CommandCondition(canExecute != null ? ((param) => canExecute()) : null);
        }

        // Mit Parameter
        public static CommandCondition Create(Func<object?, bool>? canExecute)
        {
            return new CommandCondition(canExecute);
        }

        public bool CanExecute(object? parameter = null)
            => _canExecute?.Invoke(parameter) ?? true;

        public CommandCondition Combine(params CommandCondition[] others)
        {
            var funcs = new Func<object?, bool>[others.Length + 1];
            funcs[0] = _canExecute ?? (_ => true);

            for (int i = 0; i < others.Length; i++)
                funcs[i + 1] = others[i]._canExecute ?? (_ => true);

            return new CommandCondition(p =>
            {
                for (int i = 0; i < funcs.Length; i++)
                    if (!funcs[i](p)) return false;

                return true;
            });
        }

        public CommandCondition Any(params CommandCondition[] others)
        {
            var funcs = new Func<object?, bool>[others.Length + 1];
            funcs[0] = _canExecute ?? (_ => false);

            for (int i = 0; i < others.Length; i++)
                funcs[i + 1] = others[i]._canExecute ?? (_ => false);

            return new CommandCondition(p =>
            {
                for (int i = 0; i < funcs.Length; i++)
                    if (funcs[i](p)) return true;

                return false;
            });
        }

        public CommandCondition Invert()
        {
            var func = _canExecute ?? (_ => true);
            return new CommandCondition(p => !func(p));
        }

        public static implicit operator CommandCondition(Func<object?, bool> func)
            => new CommandCondition(func);

        public static readonly CommandCondition Always = new CommandCondition(_ => true);
        public static readonly CommandCondition Never = new CommandCondition(_ => false);
    }
}