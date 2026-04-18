using System;

namespace Sachssoft.Sasopuls.ViewModels
{
    public class ViewModelInspectorPropertyMeta
    {
        public string Name { get; internal set; } = string.Empty;

        public Type PropertyType { get; internal set; } = typeof(object);

        public bool IsReadOnly { get; internal set; }

        internal protected virtual void OnInitialized()
        {
        }
    }
}
