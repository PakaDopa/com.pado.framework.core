using UnityEngine;

namespace Pado.Framework.Core.Singleton
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static bool _applicationIsQuitting;

        public static T Instance
        {
            get
            {
                if (_applicationIsQuitting)
                    return null;

                if (_instance == null)
                    _instance = FindFirstObjectByType<T>(FindObjectsInactive.Include);

                return _instance;
            }
        }

        public static bool HasInstance => !_applicationIsQuitting && Instance != null;

        public bool IsInitialized { get; protected set; }
        protected bool IsMainInstance { get; private set; }
        protected virtual bool ShouldDontDestroyOnLoad => false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStaticState()
        {
            _instance = null;
            _applicationIsQuitting = false;
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                IsMainInstance = true;

                if (ShouldDontDestroyOnLoad)
                    DontDestroyOnLoad(gameObject);

                return;
            }

            if (_instance == this)
            {
                IsMainInstance = true;
                return;
            }

            IsMainInstance = false;
            Destroy(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        protected virtual void OnApplicationQuit()
        {
            _applicationIsQuitting = true;
        }

        public virtual void Init()
        {
            IsInitialized = true;
        }
    }
}
