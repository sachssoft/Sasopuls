using System;

namespace Sachssoft.Sasopuls.Messaging
{
    public interface ICommandRule
    {
        bool CanExecute(object? parameter = null);
    }
}