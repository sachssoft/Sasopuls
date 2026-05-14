using System;
using System.Collections.Generic;

namespace Sachssoft.Sasopuls.Basic
{
    public class PropertyChangeContext
    {
        private static readonly IReadOnlyList<string> _emptyFilter = Array.Empty<string>();

        private IReadOnlyList<string>? _filter;

        public IReadOnlyList<string> Filter
        {
            get => _filter ?? _emptyFilter;
            init => _filter = value ?? _emptyFilter;
        }
    }
}
