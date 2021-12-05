using UnityEngine;

namespace Tools
{
    public abstract class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                return _instance ??= FindObjectOfType<T>();
            }
        }
    }
}