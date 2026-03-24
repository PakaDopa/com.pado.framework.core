# PADO Framework Core

가볍고 빠르게 붙일 수 있는 Unity 2D용 공통 코어 패키지.

이 패키지는 게임 전체를 통제하는 거대한 `GameManager`나 `UIManager`를 제공하지 않는다. 대신, 프로젝트를 시작할 때 거의 항상 반복되는 아래 4가지만 얇게 제공한다.

* `BootingSystem` : 코어 서비스 초기화 순서 보장
* `EventManager` : 전역 이벤트 버스
* `SaveManager` : 저장 시스템 브리지
* `Singleton / PersistentSingleton` : 공용 매니저 베이스

이 패키지는 다음 같은 사람에게 맞다.

* 짧은 기간에 여러 Unity 프로젝트를 반복 제작하는 경우
* 프로젝트마다 룰은 달라도, 초반 세팅은 비슷한 경우
* 무거운 프레임워크보다 얇은 코어를 선호하는 경우
* 이벤트 기반으로 UI / 전투 / 흐름을 느슨하게 연결하고 싶은 경우

---

## 1. 패키지 구성

```text
Runtime/
├─ Bootstrap/
│  ├─ BootingSystem.cs
│  └─ BootState.cs
├─ Events/
│  ├─ EventManager.cs
│  ├─ MEventType.cs
│  └─ EventArgs 또는 Args/
├─ Save/
│  ├─ SaveManager.cs
│  ├─ ISaveBackend.cs
│  ├─ SaveBackendBehaviour.cs
│  ├─ SaveBackendStatus.cs
│  ├─ SaveSerializationUtility.cs
│  └─ PlayerPrefsSaveBackend.cs
└─ Singleton/
   ├─ Singleton.cs
   └─ PersistentSingleton.cs
```

기본 철학은 단순하다.

* `BootingSystem`은 순서를 보장한다.
* `EventManager`는 소식을 전달한다.
* `SaveManager`는 저장 백엔드에 위임한다.
* 게임 규칙은 각 프로젝트가 가진다.

---

## 2. 빠른 시작

### 2-1. 패키지 설치

패키지를 Git URL 또는 로컬 패키지로 설치한다.

Git URL 예시:

```text
https://github.com/PakaDopa/com.pado.framework.core.git#1.0.0
```

---

## 3. 가장 먼저 해야 할 것

### 3-1. Bootstrap 씬 만들기

가장 추천하는 방식은 Bootstrap 씬을 하나 두는 것이다.

예시 씬 흐름:

* `BootstrapScene`
* `TitleScene`
* `GameScene`

`BootstrapScene`에는 아래 오브젝트 하나만 두면 된다.

* `BootingRoot`

  * `BootingSystem` 컴포넌트 추가

이 오브젝트가 시작될 때 다음을 보장한다.

* `EventManager` 존재 보장
* `SaveManager` 존재 보장
* 저장 백엔드 초기화
* 부팅 완료 이벤트 발행

---

## 4. 네임스페이스

코드에서 주로 사용하는 네임스페이스는 아래다.

```csharp
using Pado.Framework.Core.Bootstrap;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Save;
using Pado.Framework.Core.Singleton;
```

이벤트 인자 클래스가 `Args` 네임스페이스로 정리되어 있다면 추가로:

```csharp
using Pado.Framework.Core.Events.Args;
```

만약 네가 로컬에서 `EventArgs` 폴더/네임스페이스명을 따로 수정했다면, 그 부분만 맞춰서 바꾸면 된다.

---

## 5. BootingSystem 사용법

### 5-1. 역할

`BootingSystem`은 게임 규칙을 관리하지 않는다.
그 대신 코어 서비스가 안전하게 사용 가능한 상태가 되도록 초기화 순서를 고정한다.

일반적으로 기대하는 흐름은 아래와 같다.

1. `EventManager` 생성
2. `SaveManager` 생성
3. 저장 백엔드 초기화
4. `SaveSystemReady` 이벤트 발행
5. `AppBootCompleted` 이벤트 발행

### 5-2. 부팅 완료 감지 예시

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;

public class BootListenerExample : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.AppBootCompleted, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.AppBootCompleted, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        Debug.Log("[BootListenerExample] Boot completed. 이제 프로젝트 초기화를 시작해도 된다.");
    }
}
```

### 5-3. 어떤 타이밍에 무엇을 붙이면 좋나

권장 패턴:

* `BootstrapScene`에서 `BootingSystem`이 먼저 돈다.
* 이후 `TitleScene` 또는 `MainScene`으로 넘어간다.
* 각 씬의 오브젝트는 `OnEnable`에서 이벤트를 구독한다.
* UI는 현재 상태를 직접 1회 읽고, 이후 이벤트를 구독한다.

즉, `BootingSystem`은 한 번만 돌고, 그 이후 오브젝트들은 안전한 상태에서 움직이게 하는 용도다.

---

## 6. EventManager 사용법

### 6-1. EventManager의 역할

`EventManager`는 상태 저장소가 아니다.
오직 "무슨 일이 발생했다"를 전달하는 이벤트 버스다.

좋은 사용 예:

* 골드가 바뀌었다
* 스테이지가 선택되었다
* 저장이 완료되었다
* 부팅이 완료되었다

좋지 않은 사용 예:

* 현재 골드를 물어본다
* 현재 스테이지를 조회한다
* 이벤트로 데이터를 요청한다

현재 상태 조회는 각자 로컬 매니저나 데이터 소유 객체가 해야 한다.

### 6-2. 기본 이벤트 구독 예시

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;

public class StageFlowListener : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.StageSelected, OnEvent);
        EventManager.Instance.AddListener(MEventType.StageCleared, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.StageSelected, this);
        EventManager.Instance.RemoveListener(MEventType.StageCleared, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        switch (eventType)
        {
            case MEventType.StageSelected:
                Debug.Log("[StageFlowListener] Stage selected.");
                break;

            case MEventType.StageCleared:
                Debug.Log("[StageFlowListener] Stage cleared.");
                break;
        }
    }
}
```

### 6-3. 이벤트 발행 예시

```csharp
using UnityEngine;
using Pado.Framework.Core.Events;

public class StageSelector : MonoBehaviour
{
    public void SelectStage()
    {
        EventManager.Instance.PostNotification(MEventType.StageSelected, this);
    }
}
```

### 6-4. 값이 있는 이벤트 발행 예시

패키지에 `IntEventArgs`, `FloatEventArgs`, `StringEventArgs`, `BoolEventArgs` 등이 포함되어 있다면 이런 식으로 보낼 수 있다.

```csharp
using UnityEngine;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Events.Args;

public class GoldRewardGiver : MonoBehaviour
{
    [SerializeField] private int _rewardGold = 100;

    public void GiveReward()
    {
        EventManager.Instance.PostNotification(
            MEventType.GoldChanged,
            this,
            new IntEventArgs(_rewardGold));
    }
}
```

받는 쪽 예시:

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Events.Args;

public class GoldPresenterExample : MonoBehaviour
{
    private int _gold;

    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.GoldChanged, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.GoldChanged, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        IntEventArgs intArgs = args as IntEventArgs;

        if (intArgs == null)
            return;

        _gold += intArgs.Value;
        Debug.Log($"[GoldPresenterExample] Current Gold : {_gold}");
    }
}
```

### 6-5. 커스텀 이벤트 인자 만들기

프로젝트에서 필요한 데이터 구조가 있다면 새 EventArgs 클래스를 직접 만들어서 쓰면 된다.

```csharp
using System;

[Serializable]
public class StageSelectedEventArgs : EventArgs
{
    public string StageId;
    public int ChapterIndex;

    public StageSelectedEventArgs(string stageId, int chapterIndex)
    {
        StageId = stageId;
        ChapterIndex = chapterIndex;
    }
}
```

발행:

```csharp
using UnityEngine;
using Pado.Framework.Core.Events;

public class StageSelectorExample : MonoBehaviour
{
    public void SelectStage(string stageId, int chapterIndex)
    {
        EventManager.Instance.PostNotification(
            MEventType.StageSelected,
            this,
            new StageSelectedEventArgs(stageId, chapterIndex));
    }
}
```

수신:

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;

public class StageLoadController : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.StageSelected, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.StageSelected, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        StageSelectedEventArgs stageArgs = args as StageSelectedEventArgs;

        if (stageArgs == null)
            return;

        Debug.Log($"Load Stage : {stageArgs.StageId} / Chapter : {stageArgs.ChapterIndex}");
    }
}
```

### 6-6. 이벤트 사용 권장 규칙

* 구독은 `OnEnable`
* 해제는 `OnDisable`
* 상태 조회는 이벤트로 하지 말 것
* 이벤트 이름은 "발생 사실" 기준으로 만들 것
* UI는 처음 켜질 때 현재 값 1회 반영 후 이벤트를 구독할 것

---

## 7. SaveManager 사용법

### 7-1. SaveManager의 역할

`SaveManager`는 직접 게임 데이터를 소유하지 않는다.
`SaveManager`는 저장 백엔드를 통해 저장/로드를 중계하는 브리지다.

즉 이 패키지의 저장 흐름은 다음과 같다.

* 프로젝트 코드가 `SaveManager`에 저장 요청
* `SaveManager`가 현재 연결된 저장 백엔드에 위임
* 백엔드가 실제 저장 수행

현재 기본 샘플 백엔드는 `PlayerPrefsSaveBackend`다.
나중에 Easy Save, ES3, BackEnd, 커스텀 JSON, 클라우드 저장 등으로 바꾸고 싶으면 백엔드만 교체하면 된다.

### 7-2. 기본 저장 예시

```csharp
using UnityEngine;
using Pado.Framework.Core.Save;

[System.Serializable]
public class PlayerProfileData
{
    public string PlayerName;
    public int Gold;
    public int BestScore;
}

public class SaveExample : MonoBehaviour
{
    private const string SaveKey = "player_profile";

    public void SaveProfile()
    {
        PlayerProfileData data = new PlayerProfileData
        {
            PlayerName = "PakaDopa",
            Gold = 1200,
            BestScore = 9999
        };

        SaveManager.Instance.Save(SaveKey, data);
        Debug.Log("[SaveExample] Save Complete.");
    }

    public void LoadProfile()
    {
        PlayerProfileData loaded = SaveManager.Instance.Load(SaveKey, new PlayerProfileData());
        Debug.Log($"[SaveExample] {loaded.PlayerName} / {loaded.Gold} / {loaded.BestScore}");
    }
}
```

### 7-3. 값이 하나뿐인 단순 저장

```csharp
using UnityEngine;
using Pado.Framework.Core.Save;

public class SettingsSaveExample : MonoBehaviour
{
    private const string BgmVolumeKey = "settings_bgm_volume";

    public void SaveVolume(float volume)
    {
        SaveManager.Instance.Save(BgmVolumeKey, volume);
    }

    public float LoadVolume()
    {
        return SaveManager.Instance.Load(BgmVolumeKey, 1f);
    }
}
```

### 7-4. 키 존재 여부 확인

```csharp
using UnityEngine;
using Pado.Framework.Core.Save;

public class SaveCheckExample : MonoBehaviour
{
    private const string SaveKey = "player_profile";

    private void Start()
    {
        bool hasSave = SaveManager.Instance.HasKey(SaveKey);
        Debug.Log($"Has Save : {hasSave}");
    }
}
```

### 7-5. 삭제 예시

```csharp
using UnityEngine;
using Pado.Framework.Core.Save;

public class SaveDeleteExample : MonoBehaviour
{
    public void DeleteProfile()
    {
        SaveManager.Instance.Delete("player_profile");
        Debug.Log("Profile deleted.");
    }

    public void DeleteAllData()
    {
        SaveManager.Instance.DeleteAll();
        Debug.Log("All save data deleted.");
    }
}
```

### 7-6. 저장 완료 이벤트와 함께 쓰기

저장 성공/실패를 이벤트로 처리하고 싶다면, 프로젝트에서 아래처럼 연결하면 좋다.

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;

public class SaveResultListener : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.SaveCompleted, OnEvent);
        EventManager.Instance.AddListener(MEventType.SaveFailed, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.SaveCompleted, this);
        EventManager.Instance.RemoveListener(MEventType.SaveFailed, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        switch (eventType)
        {
            case MEventType.SaveCompleted:
                Debug.Log("[SaveResultListener] Save completed.");
                break;

            case MEventType.SaveFailed:
                Debug.Log("[SaveResultListener] Save failed.");
                break;
        }
    }
}
```

실제 이벤트 발행 시점은 `SaveManager` 구현에 따라 달라질 수 있으니, 현재 패키지 구현 기준으로 맞춰 사용하면 된다.

---

## 8. 외부 저장 에셋으로 교체하기

이 패키지의 핵심은 `SaveManager`를 고정하고, 저장 백엔드만 바꿀 수 있다는 점이다.

즉 게임 코드에서는 계속 이 코드만 쓴다.

```csharp
SaveManager.Instance.Save(key, data);
SaveManager.Instance.Load(key, defaultValue);
```

그리고 실제 저장 구현만 바꾼다.

### 8-1. 커스텀 저장 백엔드 예시

```csharp
using Pado.Framework.Core.Save;

public class MyExternalSaveBackend : SaveBackendBehaviour
{
    public override void Init()
    {
        // 외부 저장 에셋 초기화
        IsInitialized = true;
    }

    public override bool HasKey(string key)
    {
        // 외부 저장 에셋의 키 존재 여부 확인
        return false;
    }

    public override void Save<T>(string key, T value)
    {
        // 외부 저장 에셋 저장 호출
    }

    public override T Load<T>(string key, T defaultValue = default)
    {
        // 외부 저장 에셋 로드 호출
        return defaultValue;
    }

    public override void Delete(string key)
    {
        // 외부 저장 에셋 삭제 호출
    }

    public override void DeleteAll()
    {
        // 외부 저장 에셋 전체 삭제 호출
    }
}
```

### 8-2. 사용 방식

* `SaveManager`가 붙은 오브젝트에
* `PlayerPrefsSaveBackend` 대신
* `MyExternalSaveBackend`를 붙인다.

이렇게 하면 프로젝트 코드는 그대로 두고 저장 구현만 교체할 수 있다.

---

## 9. Singleton / PersistentSingleton 사용법

### 9-1. 언제 무엇을 써야 하나

`Singleton<T>`

* 씬 안에서 1개만 있으면 되는 매니저
* 씬이 바뀌면 같이 사라져도 되는 경우

`PersistentSingleton<T>`

* 씬 전환 후에도 유지되어야 하는 매니저
* 앱 전체에서 1개만 유지해야 하는 경우

현재 이 패키지에서는 대개 아래처럼 생각하면 된다.

* `EventManager` : `PersistentSingleton`
* `SaveManager` : `PersistentSingleton`
* `BootingSystem` : `PersistentSingleton`

### 9-2. 씬 싱글톤 예시

```csharp
using UnityEngine;
using Pado.Framework.Core.Singleton;

public class LocalStageManager : Singleton<LocalStageManager>
{
    public int CurrentEnemyCount { get; private set; }

    public override void Init()
    {
        if (IsInitialized)
            return;

        base.Init();
        CurrentEnemyCount = 0;
    }

    public void AddEnemy()
    {
        CurrentEnemyCount++;
    }
}
```

### 9-3. 전역 싱글톤 예시

```csharp
using UnityEngine;
using Pado.Framework.Core.Singleton;

public class AudioService : PersistentSingleton<AudioService>
{
    [SerializeField] private float _masterVolume = 1f;

    public float MasterVolume => _masterVolume;

    public override void Init()
    {
        if (IsInitialized)
            return;

        base.Init();
        Debug.Log("[AudioService] Init Complete.");
    }

    public void SetVolume(float volume)
    {
        _masterVolume = Mathf.Clamp01(volume);
    }
}
```

### 9-4. 권장 규칙

* 싱글톤은 존재 보장용으로만 쓸 것
* 초기화는 `BootingSystem`이나 명시적인 `Init()` 흐름에서 할 것
* 싱글톤 안에 게임 규칙을 계속 집어넣지 말 것
* 전역 참조가 너무 많아지기 시작하면 책임을 다시 쪼갤 것

---

## 10. 실제 프로젝트에서 자주 쓰는 패턴

### 10-1. 부팅 완료 후 첫 초기화

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Save;

public class TitleSceneInitializer : MonoBehaviour
{
    private const string FirstLaunchKey = "first_launch";

    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.AppBootCompleted, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.AppBootCompleted, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        bool isFirstLaunch = !SaveManager.Instance.HasKey(FirstLaunchKey);

        if (isFirstLaunch)
        {
            SaveManager.Instance.Save(FirstLaunchKey, true);
            Debug.Log("첫 실행입니다. 튜토리얼로 이동하세요.");
        }
        else
        {
            Debug.Log("기존 유저입니다. 타이틀 메뉴를 보여주세요.");
        }
    }
}
```

### 10-2. UI가 상태를 직접 읽고, 이후 이벤트만 구독하는 패턴

```csharp
using System;
using UnityEngine;
using UnityEngine.UI;
using Pado.Framework.Core.Events;
using Pado.Framework.Core.Save;
using Pado.Framework.Core.Events.Args;

public class BgmVolumeSliderPresenter : MonoBehaviour
{
    [SerializeField] private Slider _slider;
    private const string VolumeKey = "bgm_volume";

    private void OnEnable()
    {
        float savedVolume = SaveManager.Instance.Load(VolumeKey, 1f);
        _slider.value = savedVolume;

        EventManager.Instance.AddListener(MEventType.SettingsChanged, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.SettingsChanged, this);
    }

    public void OnChangedVolume(float value)
    {
        SaveManager.Instance.Save(VolumeKey, value);
        EventManager.Instance.PostNotification(MEventType.SettingsChanged, this, new FloatEventArgs(value));
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        FloatEventArgs floatArgs = args as FloatEventArgs;

        if (floatArgs == null)
            return;

        _slider.value = floatArgs.Value;
    }
}
```

이 패턴의 장점은 이렇다.

* UI가 무조건 중앙 매니저 하나에 묶이지 않음
* 현재 값은 직접 읽어서 즉시 반영 가능
* 이후 변경만 이벤트로 처리 가능

---

## 11. 자주 하는 실수

### 11-1. EventManager를 상태 저장소처럼 쓰는 것

나쁜 예:

* 현재 골드를 이벤트로 요청함
* 현재 스테이지를 이벤트로 질의함

이벤트는 알림용이고, 상태 저장은 각자의 데이터 소유 객체가 해야 한다.

### 11-2. SaveManager에 게임 규칙을 넣는 것

나쁜 예:

* `SaveManager.AddGold()`
* `SaveManager.UnlockStage()`

이건 저장 브리지의 책임이 아니다.
저장 관련 코드는 저장만, 게임 규칙은 프로젝트 로컬 매니저가 담당하는 게 맞다.

### 11-3. BootingSystem에 씬 규칙을 몰아넣는 것

나쁜 예:

* 타이틀 씬이면 뭐 하고
* 게임 씬이면 뭐 하고
* 보스 씬이면 또 다르게 하고

이렇게 되면 `BootingSystem`이 다시 작은 `GameManager`가 된다.
`BootingSystem`은 코어 초기화 순서만 담당해야 한다.

### 11-4. 이벤트 구독 해제를 빼먹는 것

구독은 `OnEnable`, 해제는 `OnDisable`로 통일하는 습관이 가장 안정적이다.

---

## 12. 추천 사용 순서

새 프로젝트를 만들었을 때는 보통 아래 순서로 붙이면 된다.

1. 패키지 설치
2. `BootstrapScene` 생성
3. `BootingSystem` 오브젝트 배치
4. 저장 백엔드 결정

   * 빠른 테스트: `PlayerPrefsSaveBackend`
   * 실제 저장: 외부 저장 에셋 백엔드
5. 프로젝트용 이벤트 enum 정리
6. 타이틀 씬 / 게임 씬에서 필요한 Presenter, Controller, Flow 오브젝트 작성
7. 현재 값 직접 반영 + 이벤트 구독 패턴으로 UI 연결

---

## 13. 최소 예제 모음

### 13-1. 저장하고 불러오기

```csharp
using UnityEngine;
using Pado.Framework.Core.Save;

public class QuickSaveExample : MonoBehaviour
{
    public void SaveInt()
    {
        SaveManager.Instance.Save("best_score", 12345);
    }

    public void LoadInt()
    {
        int bestScore = SaveManager.Instance.Load("best_score", 0);
        Debug.Log($"Best Score : {bestScore}");
    }
}
```

### 13-2. 단순 이벤트 발행 / 수신

```csharp
using System;
using UnityEngine;
using Pado.Framework.Core.Events;

public class EventQuickExample : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager.Instance.AddListener(MEventType.StageCleared, OnEvent);
    }

    private void OnDisable()
    {
        if (EventManager.Instance == null)
            return;

        EventManager.Instance.RemoveListener(MEventType.StageCleared, this);
    }

    public void ClearStage()
    {
        EventManager.Instance.PostNotification(MEventType.StageCleared, this);
    }

    private void OnEvent(MEventType eventType, Component sender, EventArgs args)
    {
        Debug.Log("Stage Cleared Event Received.");
    }
}
```

---

## 14. 마무리

이 패키지는 큰 프레임워크가 아니다.
이 패키지는 프로젝트마다 반복되는 초기 공통 세팅을 줄이기 위한 얇은 코어다.

즉 이 패키지를 가장 잘 쓰는 방법은:

* 공용 코어는 얇게 유지하고
* 프로젝트 규칙은 프로젝트 안에 두고
* 이벤트로 느슨하게 연결하고
* 저장은 SaveManager 뒤의 백엔드로 교체 가능하게 유지하는 것

이 방향을 계속 지키면, 프로젝트가 바뀌어도 초반 세팅을 빠르게 재사용할 수 있다.

---

## 15. 내 개인 규칙 메모

이 패키지를 사용할 때 스스로 지키면 좋은 규칙.

* `BootingSystem`은 절대 비대해지지 않게 유지한다.
* `EventManager`는 상태 저장소로 쓰지 않는다.
* `SaveManager`는 저장 브리지로만 유지한다.
* UI는 현재 값 1회 반영 후 이벤트를 구독한다.
* 프로젝트별 규칙은 패키지 안이 아니라 게임 코드 안에 둔다.
* 공용 패키지는 언제든 다른 게임으로 들고 갈 수 있어야 한다.

이 기준만 지켜도 이 패키지는 계속 살아남는다.
