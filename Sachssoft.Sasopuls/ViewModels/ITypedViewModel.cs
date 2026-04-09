namespace Sachssoft.Sasofly.Pulse.ViewModels
{
    public interface ITypedViewModel
    {
        bool Dirty { get; }

        bool Freeze { get; set; }

        void MarkDirty();

        void ResetDirty();

        void Reload();

        void Persist();
    }
}
