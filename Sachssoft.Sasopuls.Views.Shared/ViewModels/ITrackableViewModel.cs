namespace Sachssoft.Sasopuls.ViewModels
{
    public interface ITrackableViewModel
    {
        bool Dirty { get; }

        bool Freeze { get; set; }

        void MarkDirty();

        void ResetDirty();

        void Reload();

        void Persist();
    }
}
