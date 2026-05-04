using System;
using System.ComponentModel;

namespace Sachssoft.Sasopuls.Basic
{
    public interface INotifyPropertyChangedContext : INotifyPropertyChanged
    {
        new event EventHandler<PropertyChangedContextEventArgs>? PropertyChanged;
    }
}