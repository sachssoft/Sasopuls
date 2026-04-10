namespace Sachssoft.Sasopuls
{
    public interface IPropertyStore
    {
        T? Get<T>(string propertyName, T? fallback = default);

        void Set<T>(string propertyName, T? value);
    }
}
