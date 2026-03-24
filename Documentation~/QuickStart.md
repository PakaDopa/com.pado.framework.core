# Quick Start

## 1. Create a bootstrap scene

Create an empty scene that runs first.

## 2. Add BootingSystem

Add a GameObject with `BootingSystem` attached.

## 3. Run

When the scene starts:

- `EventManager` is ensured
- `SaveManager` is ensured
- `SaveManager.Init()` is called
- `AppBootCompleted` is fired

## 4. Listen for boot complete

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;

public class BootListener : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.AppBootCompleted, OnEvent);
    }

    private void OnDisable()
    {
        if (!EventManager.HasInstance)
            return;

        EventManager.Instance.RemoveListener(MEventType.AppBootCompleted, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        Debug.Log("Boot Complete");
    }
}
```
