namespace Sachssoft.Sasopuls.Models
{
    public interface ILocalized
    {
        string Key { get; }

        object? Fallback { get; }
    }
}