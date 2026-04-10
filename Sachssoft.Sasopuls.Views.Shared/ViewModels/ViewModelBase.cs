using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Sachssoft.Sasopuls.ViewModels
{
    public class ViewModelBase : NotifyObject, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errors = new();

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public bool HasErrors => _errors.Count > 0;

        public IEnumerable GetErrors(string? propertyName) =>
            propertyName != null && _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();

        protected void SetError(string propertyName, string error)
        {
            _errors[propertyName] = new List<string> { error };
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
