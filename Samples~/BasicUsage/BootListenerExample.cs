using System;
using UnityEngine;
using Pado.Framework.Core.Events;

namespace Pado.Framework.Samples.BasicUsage
{
    public class BootListenerExample : MonoBehaviour
    {
        private void OnEnable()
        {
            EventManager.Instance.AddListener(MEventType.AppBootCompleted, OnEvent);
            EventManager.Instance.AddListener(MEventType.SaveCompleted, OnEvent);
        }

        private void OnDisable()
        {
            if (!EventManager.HasInstance)
                return;

            EventManager.Instance.RemoveListener(MEventType.AppBootCompleted, this);
            EventManager.Instance.RemoveListener(MEventType.SaveCompleted, this);
        }

        private void OnEvent(MEventType eventType, Component sender, EventArgs args)
        {
            Debug.Log($"Received Event : {eventType}");
        }
    }
}
