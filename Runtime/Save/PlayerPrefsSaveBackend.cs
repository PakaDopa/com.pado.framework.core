using UnityEngine;

namespace Pado.Framework.Core.Save
{
    public class PlayerPrefsSaveBackend : SaveBackendBehaviour
    {
        [SerializeField] private bool _showDebugLog = false;

        public override void Init()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            if (_showDebugLog)
                Debug.Log("[PlayerPrefsSaveBackend] Init Complete.");
        }

        public override bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        public override void Save<T>(string key, T value)
        {
            string json = SaveSerializationUtility.ToJson(value);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }

        public override T Load<T>(string key, T defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key))
                return defaultValue;

            string json = PlayerPrefs.GetString(key);
            return SaveSerializationUtility.FromJson(json, defaultValue);
        }

        public override void Delete(string key)
        {
            if (!PlayerPrefs.HasKey(key))
                return;

            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        public override void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
