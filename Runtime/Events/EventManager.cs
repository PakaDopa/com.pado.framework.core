using System;
using System.Collections.Generic;
using UnityEngine;
using Pado.Framework.Core.Singleton;

namespace Pado.Framework.Core.Events
{
    public class EventManager : PersistentSingleton<EventManager>
    {
        public delegate void OnEvent(MEventType eventType, Component sender, EventArgs args = null);

        private Dictionary<MEventType, List<OnEvent>> _listeners = new Dictionary<MEventType, List<OnEvent>>();
        [SerializeField] private bool _showDebugLog = false;

        protected override void Awake()
        {
            base.Awake();

            if (!IsMainInstance)
                return;
        }

        public override void Init()
        {
            if (IsInitialized)
                return;

            IsInitialized = true;

            if (_showDebugLog)
                Debug.Log("[EventManager] Init Complete.");
        }

        public void AddListener(MEventType eventType, OnEvent listener)
        {
            if (listener == null)
                return;

            if (!_listeners.TryGetValue(eventType, out List<OnEvent> listenList))
            {
                listenList = new List<OnEvent>();
                _listeners.Add(eventType, listenList);
            }

            if (!listenList.Contains(listener))
                listenList.Add(listener);
        }

        public void RemoveListener(MEventType eventType, OnEvent listener)
        {
            if (listener == null)
                return;

            if (!_listeners.TryGetValue(eventType, out List<OnEvent> listenList))
                return;

            listenList.Remove(listener);

            if (listenList.Count == 0)
                _listeners.Remove(eventType);
        }

        public void RemoveListener(MEventType eventType, object target)
        {
            if (target == null)
                return;

            if (!_listeners.TryGetValue(eventType, out List<OnEvent> listenList))
                return;

            for (int i = listenList.Count - 1; i >= 0; i--)
            {
                OnEvent listener = listenList[i];

                if (listener == null)
                {
                    listenList.RemoveAt(i);
                    continue;
                }

                if (target == listener.Target)
                    listenList.RemoveAt(i);
            }

            if (listenList.Count == 0)
                _listeners.Remove(eventType);
        }

        public void RemoveEvent(MEventType eventType)
        {
            _listeners.Remove(eventType);
        }

        public void ClearAllListeners()
        {
            _listeners.Clear();
        }

        public void PostNotification(MEventType eventType, Component sender, EventArgs args = null)
        {
            if (!_listeners.TryGetValue(eventType, out List<OnEvent> listenList))
                return;

            if (listenList == null || listenList.Count == 0)
                return;

            OnEvent[] invokeList = listenList.ToArray();

            for (int i = 0; i < invokeList.Length; i++)
            {
                OnEvent listener = invokeList[i];

                if (IsInvalidListener(listener))
                {
                    RemoveListener(eventType, listener);
                    continue;
                }

                try
                {
                    listener.Invoke(eventType, sender, args);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[EventManager] Exception while invoking '{eventType}'.\n{ex}");
                }
            }
        }

        public void RemoveRedundancies()
        {
            Dictionary<MEventType, List<OnEvent>> newListeners = new Dictionary<MEventType, List<OnEvent>>();

            foreach (KeyValuePair<MEventType, List<OnEvent>> item in _listeners)
            {
                List<OnEvent> validListeners = new List<OnEvent>();

                for (int i = 0; i < item.Value.Count; i++)
                {
                    OnEvent listener = item.Value[i];

                    if (IsInvalidListener(listener))
                        continue;

                    validListeners.Add(listener);
                }

                if (validListeners.Count > 0)
                    newListeners.Add(item.Key, validListeners);
            }

            _listeners = newListeners;
        }

        private bool IsInvalidListener(OnEvent listener)
        {
            if (listener == null)
                return true;

            if (listener.Method != null && listener.Method.IsStatic)
                return false;

            object target = listener.Target;

            if (target == null)
                return true;

            if (target is UnityEngine.Object unityTarget && unityTarget == null)
                return true;

            return false;
        }
    }
}
