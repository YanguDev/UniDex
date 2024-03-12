using UnityEngine;

namespace UniDex.Patterns
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning($"Singleton for type {typeof(T)} already exists. Destroying {gameObject}", gameObject);
                Destroy(gameObject);
                return;
            }

            Instance = this as T;
        }
    }
}