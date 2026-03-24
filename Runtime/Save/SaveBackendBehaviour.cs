using UnityEngine;

namespace Pado.Framework.Core.Save
{
    public abstract class SaveBackendBehaviour : MonoBehaviour, ISaveBackend
    {
        public bool IsInitialized { get; protected set; }

        public abstract void Init();
        public abstract bool HasKey(string key);
        public abstract void Save<T>(string key, T value);
        public abstract T Load<T>(string key, T defaultValue = default);
        public abstract void Delete(string key);
        public abstract void DeleteAll();
    }
}
