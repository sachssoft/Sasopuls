using Sachssoft.Sasopuls.Inspector;
using System;

namespace Sachssoft.Sasopuls.Inspector
{
    public interface IInspectorProperty
    {
        event EventHandler<InspectorPropertyChangingEventArgs>? Changing;
        event EventHandler<InspectorPropertyChangedEventArgs>? Changed;

        string Name { get; }

        IInspectorSchema Schema { get; }

        InspectorPropertyMetadata Metadata { get; }

        bool IsReadOnly { get; }

        object? GetValue();

        void SetValue(object? value);
    }
}
