using System.ComponentModel;

namespace Sachssoft.Sasopuls.Basic
{
    public class PropertyChangedContextEventArgs : PropertyChangedEventArgs
    {
        public PropertyChangedContextEventArgs(
            string? propertyName,
            object? oldValue = null,
            object? newValue = null,
            PropertyChangeContext? context = null)
            : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
            Context = context ??= new PropertyChangeContext();
        }

        public object? OldValue { get; }
        public object? NewValue { get; }
        public PropertyChangeContext Context { get; }
    }
}
