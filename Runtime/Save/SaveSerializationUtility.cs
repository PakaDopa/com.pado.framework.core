using System;
using UnityEngine;

namespace Pado.Framework.Core.Save
{
    public static class SaveSerializationUtility
    {
        [Serializable]
        private class Wrapper<T>
        {
            public T Value;

            public Wrapper(T value)
            {
                Value = value;
            }
        }

        public static string ToJson<T>(T value)
        {
            Wrapper<T> wrapper = new Wrapper<T>(value);
            return JsonUtility.ToJson(wrapper);
        }

        public static T FromJson<T>(string json, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(json))
                return defaultValue;

            try
            {
                Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);

                if (wrapper == null)
                    return defaultValue;

                return wrapper.Value;
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
