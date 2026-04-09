namespace Sachssoft.Sasofly.Pulse.ViewModels
{
    public interface IViewModelPresentation
    {
        abstract object? Display { get; }

        abstract object? Description { get; }
    }
}
