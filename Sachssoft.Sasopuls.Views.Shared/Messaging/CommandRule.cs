using System;

namespace Sachssoft.Sasopuls.Messaging
{
    public sealed class CommandPolicy : ICommandRule
    {
        private readonly Func<object?, bool>? _canExecute;

        private CommandPolicy(Func<object?, bool>? canExecute)
            => _canExecute = canExecute;

        // Ohne Parameter
        public static CommandPolicy Create(Func<bool>? canExecute)
        {
            return new CommandPolicy(canExecute != null ? ((param) => canExecute()) : null);
        }

        // Mit Parameter
        public static CommandPolicy Create(Func<object?, bool>? canExecute)
        {
            return new CommandPolicy(canExecute);
        }

        public bool CanExecute(object? parameter = null)
            => _canExecute?.Invoke(parameter) ?? true;

        public CommandPolicy Combine(params CommandPolicy[] others)
        {
            var funcs = new Func<object?, bool>[others.Length + 1];
            funcs[0] = _canExecute ?? (_ => true);

            for (int i = 0; i < others.Length; i++)
                funcs[i + 1] = others[i]._canExecute ?? (_ => true);

            return new CommandPolicy(p =>
            {
                for (int i = 0; i < funcs.Length; i++)
                    if (!funcs[i](p)) return false;

                return true;
            });
        }

        public CommandPolicy Any(params CommandPolicy[] others)
        {
            var funcs = new Func<object?, bool>[others.Length + 1];
            funcs[0] = _canExecute ?? (_ => false);

            for (int i = 0; i < others.Length; i++)
                funcs[i + 1] = others[i]._canExecute ?? (_ => false);

            return new CommandPolicy(p =>
            {
                for (int i = 0; i < funcs.Length; i++)
                    if (funcs[i](p)) return true;

                return false;
            });
        }

        public CommandPolicy Invert()
        {
            var func = _canExecute ?? (_ => true);
            return new CommandPolicy(p => !func(p));
        }

        public static implicit operator CommandPolicy(Func<object?, bool> func)
            => new CommandPolicy(func);

        public static readonly CommandPolicy Always = new CommandPolicy(_ => true);
        public static readonly CommandPolicy Never = new CommandPolicy(_ => false);
    }
}