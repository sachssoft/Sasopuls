using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Sachssoft.Sasopuls.Infrastructure
{
    public class PropertyChangedFilterBuilder
    {
        private readonly List<string> _properties = new();

        private PropertyChangedFilterBuilder()
        {
        }

        public static PropertyChangedFilterBuilder Create() => new PropertyChangedFilterBuilder();

        public PropertyChangedFilterBuilder Add(string property)
        {
            _properties.Add(property);
            return this;
        }

        public PropertyChangedFilter Build(INotifyPropertyChanged source)
        {
            return new PropertyChangedFilter(source, _properties);
        }
    }
}
