using System.Collections;
using UnityEngine;

namespace NavMeshGrid
{
    public static class JsonHelper
    {
        public static T[] ArrayFromJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ArrayToJson<T>(T[] objects)
        {
            var wrapper = new Wrapper<T>();
            wrapper.Items = objects;

            return JsonUtility.ToJson(wrapper);
        }
    }

    [System.Serializable]
    public class Wrapper<T>
    {
        public T[] Items;
    }
}