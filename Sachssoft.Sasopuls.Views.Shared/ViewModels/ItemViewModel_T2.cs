namespace Sachssoft.Sasofly.Pulse.ViewModels
{
    public class ItemViewModel<TModel, T> : ViewModelBase<TModel>
    {
        private IViewModelPresentation? _presentation;
        private string? _id;
        private string? _name;
        private T? _value;
        private object? _data;
        private bool _isSelected;
        private int _index;

        public ItemViewModel(TModel model) : base(model)
        {
        }

        public IViewModelPresentation? Presentation
        {
            get => _presentation;
            set => SetAndNotify(ref _presentation, value);
        }

        public string? Id
        {
            get => _id;
            set => SetAndNotify(ref _id, value);
        }

        public string? Name
        {
            get => _name;
            set => SetAndNotify(ref _name, value);
        }

        public T? Value
        {
            get => _value;
            set => SetAndNotify(ref _value, value);
        }

        public object? Data
        {
            get => _data;
            set => SetAndNotify(ref _data, value);
        }

        public int Index
        {
            get => _index;
            set => SetAndNotify(ref _index, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetAndNotify(ref _isSelected, value);
        }
    }
}