using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Sachssoft.Sasopuls.Basic
{
    public class PropertyChangingContextEventArgs : PropertyChangingEventArgs
    {
        public PropertyChangingContextEventArgs(
            string? propertyName,
            PropertyChangeContext? context = null)
            : base(propertyName)
        {
            Context = context = new PropertyChangeContext();
        }

        public PropertyChangeContext Context { get; }
    }
}
