using Avalonia;
using Avalonia.Controls;

namespace Sachssoft.Sasopuls.Views
{
    public abstract class NotifyUserControl<TViewModel> : UserControl
        where TViewModel : NotifyObject
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "AvaloniaProperty",
            "AVP1002",
            Justification = "Generic Avalonia property is expected here.")]
        public static readonly StyledProperty<TViewModel?> ViewModelProperty =
            AvaloniaProperty.Register<NotifyUserControl<TViewModel>, TViewModel?>(
                nameof(ViewModel));

        public TViewModel? ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        protected virtual void OnViewModelEnter(TViewModel viewModel) { }

        protected virtual void OnViewModelLeave(TViewModel viewModel) { }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == DataContextProperty)
            {
                HandleViewModelChange(
                    change.OldValue as TViewModel,
                    change.NewValue as TViewModel,
                    syncViewModelProperty: true);
            }
            else if (change.Property == ViewModelProperty)
            {
                HandleViewModelChange(
                    change.OldValue as TViewModel,
                    change.NewValue as TViewModel,
                    syncDataContext: true);
            }
        }

        private void HandleViewModelChange(
            TViewModel? oldVm,
            TViewModel? newVm,
            bool syncViewModelProperty = false,
            bool syncDataContext = false)
        {
            if (ReferenceEquals(oldVm, newVm))
                return;

            if (oldVm != null)
                OnViewModelLeave(oldVm);

            if (syncViewModelProperty)
                SetCurrentValue(ViewModelProperty, newVm);

            if (syncDataContext)
                SetCurrentValue(DataContextProperty, newVm);

            if (newVm != null)
                OnViewModelEnter(newVm);
        }
    }
}