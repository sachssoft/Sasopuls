using Sachssoft.Sasopuls.ViewModels;
using System;
using System.Collections.Generic;

namespace Sachssoft.Sasopuls.ViewModels
{
    public class ViewModelInspector
    {
        private readonly ViewModelBase _owner;
        private readonly Dictionary<string, object> _properties = new();

        public ViewModelInspector(ViewModelBase owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public ViewModelBase Owner => _owner;

        public void AddProperty<T>(
            string name,
            Func<T?> getter,
            Action<T?>? setter = null,
            ViewModelInspectorPropertyMeta? meta = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Property name cannot be null or empty.", nameof(name));

            if (getter is null)
                throw new ArgumentNullException(nameof(getter));

            if (_properties.ContainsKey(name))
                throw new InvalidOperationException($"Property '{name}' is already registered.");

            meta ??= new ViewModelInspectorPropertyMeta();

            meta.Name = name;
            meta.PropertyType = typeof(T);
            meta.IsReadOnly = setter == null;

            _properties[name] = CreateProperty<T>(getter, setter, meta);

            meta.OnInitialized();
        }

        protected virtual ViewModelInspectorProperty<T> CreateProperty<T>(
            Func<T?> getter,
            Action<T?>? setter,
            ViewModelInspectorPropertyMeta meta)
        {
            return new ViewModelInspectorProperty<T>(this, getter, setter, meta);
        }

        public bool TryGet(string name, out object? value)
        {
            if (_properties.TryGetValue(name, out var entry))
            {
                value = entry;
                return true;
            }

            value = null;
            return false;
        }

        public bool TryGet<T>(string name, out ViewModelInspectorProperty<T>? property)
        {
            if (_properties.TryGetValue(name, out var entry) &&
                entry is ViewModelInspectorProperty<T> typed)
            {
                property = typed;
                return true;
            }

            property = null;
            return false;
        }

        public IEnumerable<KeyValuePair<string, object>> GetAll()
            => _properties;
    }
}