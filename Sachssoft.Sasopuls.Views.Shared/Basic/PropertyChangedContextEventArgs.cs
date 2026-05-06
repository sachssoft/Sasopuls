using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

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
