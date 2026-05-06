using System;
using System.Collections.Generic;

namespace Sachssoft.Sasopuls.Inspector
{
    public class InspectorSchemaBuilder<TOwner>
        where TOwner : class
    {
        private readonly TOwner _owner;
        private readonly Dictionary<string, Func<InspectorSchema<TOwner>, IInspectorProperty>> _propertyFactories = new();

        private bool _built;

        public InspectorSchemaBuilder(TOwner owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public TOwner Owner => _owner;

        public void AddProperty<T>(
            string name,
            Func<TOwner, T?> getter,
            Action<TOwner, T?>? setter = null,
            InspectorPropertyMetadata? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Property name cannot be null or empty.", nameof(name));

            if (getter is null)
                throw new ArgumentNullException(nameof(getter));

            if (_propertyFactories.ContainsKey(name))
                throw new InvalidOperationException($"Property '{name}' is already registered.");

            metadata ??= CreatePropertyMeta() ?? new InspectorPropertyMetadata();
            _propertyFactories[name] = (scheme) => CreateProperty(scheme, name, getter, setter, metadata);
        }

        protected virtual InspectorPropertyMetadata CreatePropertyMeta()
            => new InspectorPropertyMetadata();

        protected virtual InspectorProperty<TOwner, T> CreateProperty<T>(
            InspectorSchema<TOwner> scheme,
            string name,
            Func<TOwner, T?> getter,
            Action<TOwner, T?>? setter,
            InspectorPropertyMetadata metadata)
        {
            return new InspectorProperty<TOwner, T>(scheme, name, getter, setter, metadata);
        }

        public InspectorSchema<TOwner> Build()
        {
            if (_built)
                throw new InvalidOperationException("Already built.");

            _built = true;

            return new InspectorSchema<TOwner>(_owner, _propertyFactories);
        }
    }
}