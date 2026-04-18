using System;

namespace Sachssoft.Sasopuls.Messaging
{
    public interface ICommandRule
    {
        event EventHandler? CanExecuteChanged;

        bool CanExecute(object? parameter = null);
    }
}