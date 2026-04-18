using Sachssoft.Sasopuls.ViewModels;

namespace Sachssoft.Sasopuls.Test
{
    static class InspectorExample
    {
        public class CatVM : ViewModelBase, IViewModelInspectorProvider
        {
            private readonly ViewModelInspector _inspector;
            private string _name;
            private int _age;

            public CatVM()
            {
                _inspector = new ViewModelInspector(this);

                _inspector.AddProperty<string>(
                    nameof(Name),
                    () => Name,
                    v => Name = v
                );

                _inspector.AddProperty<int>(
                    nameof(Age),
                    () => Age,
                    v => Age = v
                );
            }

            public ViewModelInspector Inspector => _inspector;

            public string Name
            {
                get => _name;
                set => SetAndNotify(ref _name, value);
            }

            public int Age
            {
                get => _age;
                set => SetAndNotify(ref _age, value);
            }
        }
    }
}