using UnityEngine;
using Pado.Framework.Core.Save;

namespace Pado.Framework.Samples.BasicUsage
{
    [System.Serializable]
    public class SampleSaveData
    {
        public int gold;
        public string playerName;
    }

    public class SaveExample : MonoBehaviour
    {
        private const string SaveKey = "sample_profile";

        public void SaveData()
        {
            SampleSaveData data = new SampleSaveData
            {
                gold = 100,
                playerName = "Kim"
            };

            SaveManager.Instance.Save(SaveKey, data);
        }

        public void LoadData()
        {
            SampleSaveData data = SaveManager.Instance.Load(SaveKey, new SampleSaveData());
            Debug.Log($"{data.playerName} / {data.gold}");
        }
    }
}
