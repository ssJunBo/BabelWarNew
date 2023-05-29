using System;
using UnityEngine;

namespace _GameBase
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance==null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    obj.AddComponent<T>();
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = (T)this;
            }
            else
            {
                Debug.LogError("Get a second instance of this class :" + GetType());
            }
        }

        protected virtual void OnDestroy()
        {
            _instance = null;
        }
    }
}
