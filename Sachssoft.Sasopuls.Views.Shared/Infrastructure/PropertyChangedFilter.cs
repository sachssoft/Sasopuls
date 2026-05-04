using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sachssoft.Sasopuls.Infrastructure
{
    public class PropertyChangedFilter : INotifyPropertyChanged, IDisposable
    {
        private readonly HashSet<string> _properties;
        private readonly INotifyPropertyChanged _source;

        public event PropertyChangedEventHandler? PropertyChanged;

        public PropertyChangedFilter(
            INotifyPropertyChanged source,
            IEnumerable<string> properties)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _properties = new HashSet<string>(properties ?? throw new ArgumentNullException(nameof(properties)));

            _source.PropertyChanged += OnSourcePropertyChanged;
        }

        private void OnSourcePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || _properties.Contains(e.PropertyName))
            {
                PropertyChanged?.Invoke(this, e);
            }
        }

        public void Dispose()
        {
            _source.PropertyChanged -= OnSourcePropertyChanged;
        }
    }
}