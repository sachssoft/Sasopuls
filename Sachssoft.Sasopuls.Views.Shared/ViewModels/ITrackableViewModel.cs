namespace Sachssoft.Sasopuls.ViewModels
{
    public interface ITrackableViewModel
    {
        bool IsDirty { get; }

        bool IsFrozen { get; set; }

        void MarkDirty();

        void ResetDirty();

        void Reload();

        void Persist();
    }
}
