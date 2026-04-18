using System;

namespace Sachssoft.Sasopuls.ViewModels
{
    public class ViewModelInspectorProperty<T> : IViewModelInspectorProperty
    {
        public ViewModelInspectorProperty(
            ViewModelInspector inspector,
            Func<T?> getter,
            Action<T?>? setter,
            ViewModelInspectorPropertyMeta meta)
        {
            Inspector = inspector ?? throw new ArgumentNullException(nameof(inspector));
            Getter = getter ?? throw new ArgumentNullException(nameof(getter));
            Setter = setter;
            Meta = meta ?? throw new ArgumentNullException(nameof(meta));
        }

        public ViewModelInspector Inspector { get; }

        public ViewModelInspectorPropertyMeta Meta { get; }

        public Func<T?> Getter { get; }

        public Action<T?>? Setter { get; }

        public bool IsReadOnly => Setter == null;

        // -------------------------
        // Typed API
        // -------------------------
        public T? GetValue()
        {
            return OnGetting();
        }

        public void SetValue(T? value)
        {
            if (IsReadOnly)
                return;

            OnSetting(value);
        }

        // -------------------------
        // Interface API
        // -------------------------
        object? IViewModelInspectorProperty.GetValue()
        {
            return GetValue();
        }

        void IViewModelInspectorProperty.SetValue(object? value)
        {
            if (IsReadOnly)
                return;

            if (value is null)
            {
                SetValue(default);
                return;
            }

            if (value is T typed)
            {
                SetValue(typed);
                return;
            }

            throw new InvalidOperationException(
                $"Invalid value type '{value.GetType()}' for property '{Meta.Name}'. Expected '{typeof(T)}'.");
        }

        // -------------------------
        // Interception Layer
        // -------------------------
        protected virtual T? OnGetting()
        {
            return Getter();
        }

        protected virtual void OnSetting(T? value)
        {
            Setter?.Invoke(value);
        }
    }
}