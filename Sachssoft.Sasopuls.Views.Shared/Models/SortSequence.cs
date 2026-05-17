using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sachssoft.Sasopuls.Models
{
    public class SortSequence<T> : IReadOnlyList<SortSequenceItem<T>>
    {
        private readonly List<SortSequenceItem<T>> _items;
        private readonly Dictionary<string, SortSequenceItem<T>> _itemsByName;

        public SortSequence() : this(Array.Empty<SortSequenceItem<T>>())
        { }

        public SortSequence(IEnumerable<SortSequenceItem<T>> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            _items = items.ToList();
            _itemsByName = new Dictionary<string, SortSequenceItem<T>>(StringComparer.Ordinal);

            foreach (var item in _items)
            {
                if (item == null)
                    throw new ArgumentException("SortSequence contains a null item.");

                if (string.IsNullOrWhiteSpace(item.Name))
                    throw new ArgumentException("SortSequence contains an item with an invalid or empty Name.");

                if (_itemsByName.ContainsKey(item.Name))
                    throw new ArgumentException($"Duplicate Name in SortSequence: '{item.Name}'");

                _itemsByName[item.Name] = item;
            }
        }

        public SortSequenceItem<T> this[int index] => _items[index];

        public SortSequenceItem<T> this[string name]
        {
            get
            {
                if (!_itemsByName.TryGetValue(name, out var item))
                    throw new KeyNotFoundException($"No SortSequenceItem found with Name '{name}'.");

                return item;
            }
        }

        public int Count => _items.Count;

        public SortSequenceItem<T> GetByName(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (!_itemsByName.TryGetValue(name, out var item))
                throw new KeyNotFoundException($"No SortSequenceItem found with Name '{name}'.");

            return item;
        }

        public bool TryGetByName(string name, out SortSequenceItem<T> item)
        {
            if (name == null)
            {
                item = null;
                return false;
            }

            return _itemsByName.TryGetValue(name, out item);
        }

        public IEnumerator<SortSequenceItem<T>> GetEnumerator()
            => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}