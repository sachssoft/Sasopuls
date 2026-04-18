using System;
using System.Collections.Generic;
using System.Text;

namespace Sachssoft.Sasopuls.ViewModels
{
    public interface IViewModelInspectorProperty
    {
        ViewModelInspector Inspector { get; }

        ViewModelInspectorPropertyMeta Meta { get; }

        bool IsReadOnly { get; }

        object? GetValue();

        void SetValue(object? value);
    }
}
