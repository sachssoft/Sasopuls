using System;
using System.ComponentModel;

namespace Sachssoft.Sasopuls.Basic
{
    public interface INotifyPropertyChangingContext : INotifyPropertyChanging
    {
        new event EventHandler<PropertyChangingContextEventArgs>? PropertyChanging;
    }
}