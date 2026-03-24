using UnityEngine;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Events.EventArgs;
using Pado.Framework.Core.Save;
using Pado.Framework.Core.Singleton;

namespace Pado.Framework.Core.Bootstrap
{
    public class BootingSystem : PersistentSingleton<BootingSystem>
    {
        [Header("Boot")]
        [SerializeField] private bool _bootOnStart = true;
        [SerializeField] private bool _autoCreateMissingServices = true;
        [SerializeField] private bool _autoAddPlayerPrefsBackendWhenMissing = true;

        [Header("Debug")]
        [SerializeField] private bool _showDebugLog = true;

        public BootState CurrentState { get; private set; } = BootState.None;
        public bool IsBootCompleted { get; private set; }

        protected override void Awake()
        {
            base.Awake();

            if (!IsMainInstance)
                return;
        }

        private void Start()
        {
            if (_bootOnStart)
                Boot();
        }

        public void Boot()
        {
            if (IsBootCompleted)
                return;

            CurrentState = BootState.CreatingCoreServices;
            EnsureCoreServices();

            CurrentState = BootState.InitializingEventManager;
            EventManager.Instance.Init();
            EventManager.Instance.PostNotification(MEventType.AppBootStarted, this, EmptyEventArgs.Instance);
            EventManager.Instance.PostNotification(MEventType.EventSystemReady, this, EmptyEventArgs.Instance);

            CurrentState = BootState.InitializingSaveManager;
            SaveManager.Instance.Init();

            if (!SaveManager.Instance.IsReady)
            {
                CurrentState = BootState.Failed;
                EventManager.Instance.PostNotification(MEventType.AppBootFailed, this, EmptyEventArgs.Instance);

                if (_showDebugLog)
                    Debug.LogError("[BootingSystem] Boot failed. SaveManager is not ready.");

                return;
            }

            EventManager.Instance.PostNotification(MEventType.SaveSystemReady, this, EmptyEventArgs.Instance);

            CurrentState = BootState.Completed;
            IsBootCompleted = true;
            EventManager.Instance.PostNotification(MEventType.AppBootCompleted, this, EmptyEventArgs.Instance);

            if (_showDebugLog)
                Debug.Log("[BootingSystem] Boot Complete.");
        }

        private void EnsureCoreServices()
        {
            EnsureEventManager();
            EnsureSaveManager();
        }

        private void EnsureEventManager()
        {
            if (EventManager.Instance != null)
                return;

            if (!_autoCreateMissingServices)
                return;

            GameObject eventManagerObject = new GameObject("EventManager");
            eventManagerObject.AddComponent<EventManager>();
        }

        private void EnsureSaveManager()
        {
            SaveManager saveManager = SaveManager.Instance;

            if (saveManager == null && _autoCreateMissingServices)
            {
                GameObject saveManagerObject = new GameObject("SaveManager");
                saveManager = saveManagerObject.AddComponent<SaveManager>();
            }

            if (saveManager == null)
                return;

            if (saveManager.BackendBehaviour != null)
                return;

            if (!_autoAddPlayerPrefsBackendWhenMissing)
                return;

            PlayerPrefsSaveBackend fallbackBackend = saveManager.GetComponent<PlayerPrefsSaveBackend>();

            if (fallbackBackend == null)
                fallbackBackend = saveManager.gameObject.AddComponent<PlayerPrefsSaveBackend>();

            saveManager.AssignBackendIfEmpty(fallbackBackend);
        }
    }
}
