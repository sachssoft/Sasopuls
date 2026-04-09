using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Sachssoft.Sasofly.Pulse.ViewModels
{
    /// <summary>
    /// Selector for ViewModels, supports either source models or factories from a registry.
    /// </summary>
    public sealed class ViewModelSelector : NotifyObject
    {
        private readonly ReadOnlyObservableCollection<ViewModelBase> _availableItemsReadOnly;
        private ViewModelBase? _selectedItem;
        private readonly bool _allowNull;

        /// <summary>
        /// Immutable list of available ViewModels
        /// </summary>
        public ReadOnlyObservableCollection<ViewModelBase> AvailableItems => _availableItemsReadOnly;

        /// <summary>
        /// Constructor using a registry of ViewModel factories
        /// </summary>
        public ViewModelSelector(ViewModelFactoryRegistry registry, bool allowNull = true)
        {
            if (registry is null)
                throw new ArgumentNullException(nameof(registry));

            _allowNull = allowNull;

            var factories = registry.AvailableFactories
                                    .Where(f => typeof(ViewModelBase).IsAssignableFrom(f.ModelType))
                                    .ToList();

            var items = factories
                        .Select(f =>
                        {
                            var model = f.ProvideModel() as ViewModelBase
                                        ?? throw new InvalidOperationException($"Factory for {f.ModelType.Name} cannot provide a ViewModelBase model.");
                            return (ViewModelBase)f.Build(model);
                        })
                        .ToList();

            _availableItemsReadOnly = new ReadOnlyObservableCollection<ViewModelBase>(
                                        new ObservableCollection<ViewModelBase>(items));

            if (!_allowNull && _availableItemsReadOnly.Count > 0)
                _selectedItem = _availableItemsReadOnly[0];
        }

        public bool AllowNull => _allowNull;

        public bool IsSet => _selectedItem != null;

        public ViewModelBase? SelectedItem
        {
            get => _selectedItem;
            set
            {
                ViewModelBase? newValue = value;

                if (!_allowNull && newValue == null)
                    newValue = _availableItemsReadOnly.FirstOrDefault();

                if (newValue != null)
                    _selectedItem = newValue;

                RaisePropertyChanged(nameof(SelectedItem));
                RaisePropertyChanged(nameof(IsSet));
            }
        }

        public void SelectItemByIndex(int index)
        {
            if (_availableItemsReadOnly.Count == 0)
                return;

            var clampedIndex = Math.Clamp(index, 0, _availableItemsReadOnly.Count - 1);
            SelectedItem = _availableItemsReadOnly[clampedIndex];
        }
    }
}
