# PADO Framework Core

A lightweight Unity package for rapid 2D prototype setup.

## Included

- BootingSystem
- EventManager
- SaveManager
- Singleton / PersistentSingleton
- Default PlayerPrefs save backend

## Folder layout

- `Runtime/Bootstrap`
- `Runtime/Events`
- `Runtime/Save`
- `Runtime/Singleton`

## Install

### Local package

1. Copy this folder anywhere outside or inside your Unity project.
2. Open Unity.
3. Go to `Window > Package Manager`.
4. Click `+`.
5. Choose `Install package from disk...`.
6. Select this package's `package.json`.

### Git package

Push this folder to a Git repository and add it to `Packages/manifest.json`.

```json
{
  "dependencies": {
    "com.pado.framework.core": "https://github.com/yourname/com.pado.framework.core.git#1.0.0"
  }
}
```

## Basic setup

1. Create a bootstrap scene.
2. Add a `BootingSystem` GameObject.
3. Run the scene.
4. If no save backend is assigned, `PlayerPrefsSaveBackend` is added automatically when fallback is enabled.

## Replacing the save backend

Create a new class that inherits from `SaveBackendBehaviour`, then attach it to the same GameObject as `SaveManager`.

```csharp
using Pado.Framework.Core.Save;

public class MyExternalSaveBackend : SaveBackendBehaviour
{
    public override void Init()
    {
        IsInitialized = true;
    }

    public override bool HasKey(string key) => false;
    public override void Save<T>(string key, T value) { }
    public override T Load<T>(string key, T defaultValue = default) => defaultValue;
    public override void Delete(string key) { }
    public override void DeleteAll() { }
}
```

## Notes

- `BootingSystem` guarantees initialization order.
- `EventManager` only broadcasts events. It should not store gameplay state.
- `SaveManager` is designed as a thin adapter over your real save solution.
