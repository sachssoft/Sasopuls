using Sachssoft.Sasopuls.ViewModels;
using System.ComponentModel;

namespace Sachssoft.Sasopuls.Test
{
    class AnimalViewModel : ModelViewModelBase<AnimalModel>
    {
        public AnimalViewModel(AnimalModel model) : base(model)
        {

        }

        public string? Name
        {
            get => Model.Name;
            set => SetAndNotify(() => Model.Name, (v) => Model.Name = v, value);
        }

        public string? Type
        {
            get => Model.Type;
            set => SetAndNotify(() => Model.Type, (v) => Model.Type = v, value);
        }

    }

}
