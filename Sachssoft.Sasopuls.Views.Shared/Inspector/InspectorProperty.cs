using System;

namespace Sachssoft.Sasopuls.Inspector
{
    public class InspectorProperty : IInspectorProperty
    {
        private readonly Type _type;

        public event EventHandler<InspectorPropertyChangingEventArgs>? Changing;
        public event EventHandler<InspectorPropertyChangedEventArgs>? Changed;

        public InspectorProperty(
            InspectorSchema schema,
            string name,
            Type type,
            Func<object, Type, object?> getter,
            Action<object, Type, object?>? setter,
            InspectorPropertyMetadata meta)
        {
            Schema = schema ?? throw new ArgumentNullException(nameof(schema));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _type = type ?? throw new ArgumentNullException(nameof(type));
            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
            Setter = setter;
            Metadata = meta ?? throw new ArgumentNullException(nameof(meta));

            Metadata.Setup(this);
        }

        public string Name { get; }

        public InspectorSchema Schema { get; }

        IInspectorSchema IInspectorProperty.Schema => Schema;

        public InspectorPropertyMetadata Metadata { get; }

        public Func<object, Type, object?> Getter { get; }

        public Action<object, Type, object?>? Setter { get; }

        public bool IsReadOnly => Setter == null;

        public object? GetValue()
        {
            var value = OnGetting(_type);

            if (value != null && !_type.IsInstanceOfType(value))
            {
                throw new InvalidOperationException(
                    $"Getter returned '{value.GetType()}', expected '{_type}' for '{Name}'.");
            }

            return value;
        }

        public void SetValue(object? value)
        {
            if (IsReadOnly)
                return;

            if (value != null && !_type.IsInstanceOfType(value))
            {
                throw new InvalidOperationException(
                    $"Invalid value type '{value.GetType()}' for '{Name}'. Expected '{_type}'.");
            }

            var changingEventArgs = new InspectorPropertyChangingEventArgs(this);
            OnChanging(changingEventArgs);
            if (changingEventArgs.Cancel)
                return;

            OnSetting(_type, value);

            OnChanged(new InspectorPropertyChangedEventArgs(this));
        }

        protected virtual object? OnGetting(Type type)
        {
            return Getter.Invoke(Schema.Owner, type);
        }

        protected virtual void OnSetting(Type type, object? value)
        {
            Setter?.Invoke(Schema.Owner, type, value);
        }

        protected virtual void OnChanging(InspectorPropertyChangingEventArgs e)
            => Changing?.Invoke(Schema, e);

        protected virtual void OnChanged(InspectorPropertyChangedEventArgs e)
            => Changed?.Invoke(Schema, e);
    }
}