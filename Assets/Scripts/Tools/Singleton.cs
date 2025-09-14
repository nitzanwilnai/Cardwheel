using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools
{
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {

        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                        Debug.LogErrorFormat("Trying to access an uninitialized MonoBehaviourSingleton!!!");
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

    }
}