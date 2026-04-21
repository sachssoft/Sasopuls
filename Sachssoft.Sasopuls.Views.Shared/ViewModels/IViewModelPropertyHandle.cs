namespace Sachssoft.Sasopuls.ViewModels
{
    public interface IViewModelInspectorProperty
    {
        ViewModelInspector Inspector { get; }

        ViewModelInspectorPropertyMeta Meta { get; }

        bool IsReadOnly { get; }

        object? GetValue();

        void SetValue(object? value);
    }
}
