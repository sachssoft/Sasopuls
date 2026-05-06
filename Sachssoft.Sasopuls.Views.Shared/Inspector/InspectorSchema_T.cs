using System;
using System.Collections;
using System.Collections.Generic;

namespace Sachssoft.Sasopuls.Inspector
{
    public sealed class InspectorSchema<TOwner> : IInspectorSchema
        where TOwner : class
    {
        private readonly TOwner _owner;
        private readonly IReadOnlyDictionary<string, IInspectorProperty> _properties;

        public InspectorSchema(TOwner owner, IEnumerable<IInspectorProperty> properties)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            var dict = new Dictionary<string, IInspectorProperty>();

            foreach (var property in properties)
            {
                if (property == null)
                    throw new InvalidOperationException("Property cannot be null.");

                var name = property.Name;
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidOperationException("Property name is invalid.");

                if (!dict.TryAdd(name, property))
                    throw new InvalidOperationException($"Duplicate property '{name}'.");
            }

            _properties = dict;
        }

        internal InspectorSchema(
            TOwner owner,
            Dictionary<string, Func<InspectorSchema<TOwner>, IInspectorProperty>> propertyFactories)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));

            var dict = new Dictionary<string, IInspectorProperty>(propertyFactories.Count);

            foreach (var kv in propertyFactories)
            {
                var factory = kv.Value;
                var property = factory.Invoke(this);
                var name = property.Name ?? kv.Key;

                dict[name] = property;
            }

            _properties = dict;
        }

        public TOwner Owner => _owner;

        public IReadOnlyDictionary<string, IInspectorProperty> Properties => _properties;

        public bool TryGet(string name, out IInspectorProperty? property)
            => _properties.TryGetValue(name, out property);

        public bool TryGet<T>(string name, out InspectorProperty<TOwner, T>? property)
        {
            if (_properties.TryGetValue(name, out var entry) &&
                entry is InspectorProperty<TOwner, T> typed)
            {
                property = typed;
                return true;
            }

            property = null;
            return false;
        }

        public IEnumerator<IInspectorProperty> GetEnumerator()
            => _properties.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}