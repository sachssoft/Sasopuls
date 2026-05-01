using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Sachssoft.Sasopuls.Messaging
{
    public abstract class CommandBase : ICommand, IInvalidateable
    {
        public event EventHandler? CanExecuteChanged;

        public abstract bool CanExecute(object? parameter);

        public abstract void Execute(object? parameter);

        public void Invalidate()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        [Obsolete("Use Invalidate() instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
