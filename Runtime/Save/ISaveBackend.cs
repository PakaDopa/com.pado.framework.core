namespace Pado.Framework.Core.Save
{
    public interface ISaveBackend
    {
        bool IsInitialized { get; }
        void Init();
        bool HasKey(string key);
        void Save<T>(string key, T value);
        T Load<T>(string key, T defaultValue = default);
        void Delete(string key);
        void DeleteAll();
    }
}
