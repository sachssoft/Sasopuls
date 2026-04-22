using System;
using System.Collections.Generic;
using System.Linq;

namespace Sachssoft.Sasopuls.ViewModels
{
    public class ViewModelInspector
    {
        private readonly ViewModelBase _owner;
        private readonly Dictionary<string, IViewModelInspectorProperty> _properties = new();

        public ViewModelInspector(ViewModelBase owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public ViewModelBase Owner => _owner;

        // version 1.2.0:
        // add schema (IINspectorSchema)

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

            _properties[name] = CreateProperty(getter, setter, meta);

            meta.OnInitialized();
        }

        public void AddProperty<T>(
            string name,
            Func<T?> getter)
            => AddProperty(name, getter, null, null);

        public void AddProperty(
            string name,
            Func<object?> getter,
            Action<object?>? setter = null,
            ViewModelInspectorPropertyMeta? meta = null)
        {
            if (getter is null)
                throw new ArgumentNullException(nameof(getter));

            AddProperty<object?>(name, getter, setter, meta);
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

        public IEnumerable<IViewModelInspectorProperty> GetProperties()
            => _properties.Values;

        [Obsolete("Use GetProperties() instead. This member may be removed in a future version.")]
        public IEnumerable<KeyValuePair<string, object>> GetAll()
            => _properties.Select(x => new KeyValuePair<string, object>(x.Key, x.Value));
    }
}