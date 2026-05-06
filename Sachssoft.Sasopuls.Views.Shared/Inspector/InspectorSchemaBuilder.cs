using System;
using System.Collections.Generic;

namespace Sachssoft.Sasopuls.Inspector
{
    public class InspectorSchemaBuilder
    {
        private readonly object _owner;
        private readonly Dictionary<string, Func<InspectorSchema, IInspectorProperty>> _propertyFactories = new();

        private bool _built;

        public InspectorSchemaBuilder(object owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public object Owner => _owner;

        public void AddProperty(
            Type type,
            string name,
            Func<object, Type, object?> getter,
            Action<object, Type, object?>? setter = null,
            InspectorPropertyMetadata? metadata = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Property name cannot be null or empty.", nameof(name));

            if (getter is null)
                throw new ArgumentNullException(nameof(getter));

            if (_propertyFactories.ContainsKey(name))
                throw new InvalidOperationException($"Property '{name}' is already registered.");

            metadata ??= CreatePropertyMeta() ?? new InspectorPropertyMetadata();
            _propertyFactories[name] = (scheme) => CreateProperty(scheme, name, type, getter, setter, metadata);
        }

        protected virtual InspectorPropertyMetadata CreatePropertyMeta()
            => new InspectorPropertyMetadata();

        protected virtual InspectorProperty CreateProperty(
            InspectorSchema scheme,
            string name,
            Type type,
            Func<object, Type, object?> getter,
            Action<object, Type, object?>? setter,
            InspectorPropertyMetadata metadata)
        {
            return new InspectorProperty(scheme, name, type, getter, setter, metadata);
        }

        public InspectorSchema Build()
        {
            if (_built)
                throw new InvalidOperationException("Already built.");

            _built = true;

            return new InspectorSchema(_owner, _propertyFactories);
        }
    }
}