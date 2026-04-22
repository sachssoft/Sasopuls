using System;

namespace Sachssoft.Sasopuls.ViewModels
{
    public interface IViewModelTypeProvider
    {
        Type ViewModelType { get; }
    }
}
