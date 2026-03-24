using UnityEngine;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Events.EventArgs;
using Pado.Framework.Core.Singleton;

namespace Pado.Framework.Core.Save
{
    public class SaveManager : PersistentSingleton<SaveManager>
    {
        [Header("Backend")]
        [SerializeField] private SaveBackendBehaviour _backendBehaviour;

        [Header("Debug")]
        [SerializeField] private bool _showDebugLog = false;

        public SaveBackendStatus Status { get; private set; } = SaveBackendStatus.None;
        public bool IsReady => Status == SaveBackendStatus.Ready && _backendBehaviour != null && _backendBehaviour.IsInitialized;
        public SaveBackendBehaviour BackendBehaviour => _backendBehaviour;

        protected override void Awake()
        {
            base.Awake();

            if (!IsMainInstance)
                return;

            TryResolveBackend();
        }

        private void Reset()
        {
            TryResolveBackend();
        }

        public override void Init()
        {
            if (IsReady)
                return;

            Status = SaveBackendStatus.Initializing;
            TryResolveBackend();

            if (_backendBehaviour == null)
            {
                Status = SaveBackendStatus.Failed;
                Debug.LogError("[SaveManager] Save backend is missing. Attach a SaveBackendBehaviour to the same GameObject.");
                return;
            }

            _backendBehaviour.Init();

            if (!_backendBehaviour.IsInitialized)
            {
                Status = SaveBackendStatus.Failed;
                Debug.LogError("[SaveManager] Save backend init failed.");
                return;
            }

            Status = SaveBackendStatus.Ready;
            IsInitialized = true;

            if (_showDebugLog)
                Debug.Log($"[SaveManager] Init Complete. Backend = {_backendBehaviour.GetType().Name}");
        }

        public void SetBackend(SaveBackendBehaviour backendBehaviour)
        {
            _backendBehaviour = backendBehaviour;
        }

        public void AssignBackendIfEmpty(SaveBackendBehaviour backendBehaviour)
        {
            if (_backendBehaviour != null)
                return;

            _backendBehaviour = backendBehaviour;
        }

        public bool HasKey(string key)
        {
            if (!ValidateReadyState())
                return false;

            return _backendBehaviour.HasKey(key);
        }

        public void Save<T>(string key, T value)
        {
            if (!ValidateReadyState())
            {
                PublishFailureEvent(key);
                return;
            }

            _backendBehaviour.Save(key, value);
            PublishSuccessEvent(key);
        }

        public T Load<T>(string key, T defaultValue = default)
        {
            if (!ValidateReadyState())
                return defaultValue;

            return _backendBehaviour.Load(key, defaultValue);
        }

        public void Delete(string key)
        {
            if (!ValidateReadyState())
                return;

            _backendBehaviour.Delete(key);

            if (EventManager.HasInstance)
                EventManager.Instance.PostNotification(MEventType.SaveDeleted, this, new StringEventArgs(key));
        }

        public void DeleteAll()
        {
            if (!ValidateReadyState())
                return;

            _backendBehaviour.DeleteAll();

            if (EventManager.HasInstance)
                EventManager.Instance.PostNotification(MEventType.SaveDeletedAll, this, EmptyEventArgs.Instance);
        }

        private void TryResolveBackend()
        {
            if (_backendBehaviour != null)
                return;

            _backendBehaviour = GetComponent<SaveBackendBehaviour>();

            if (_backendBehaviour == null)
                _backendBehaviour = GetComponentInChildren<SaveBackendBehaviour>(true);
        }

        private bool ValidateReadyState()
        {
            if (IsReady)
                return true;

            Debug.LogError("[SaveManager] SaveManager is not ready. Call Init() through BootingSystem first.");
            return false;
        }

        private void PublishSuccessEvent(string key)
        {
            if (!EventManager.HasInstance)
                return;

            EventManager.Instance.PostNotification(MEventType.SaveCompleted, this, new StringEventArgs(key));
        }

        private void PublishFailureEvent(string key)
        {
            if (!EventManager.HasInstance)
                return;

            EventManager.Instance.PostNotification(MEventType.SaveFailed, this, new StringEventArgs(key));
        }
    }
}
